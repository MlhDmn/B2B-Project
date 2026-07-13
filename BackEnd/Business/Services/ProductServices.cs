using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using B2B_Proje.DataAccess.Context;
using B2B_Proje.DataAccess.Entities; 
using B2B_Proje.DataAccess.Enums;
using B2B_Proje.Business.DTOs.ProductDTOs;

namespace B2B_Proje.Business.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<PagedProductsResponseDto> GetAllProductsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? categoryId = null,
            ProductGender? gender = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool inStockOnly = false)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            var normalizedSearchTerm = searchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
            {
                var searchPattern = $"%{normalizedSearchTerm}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, searchPattern) ||
                    EF.Functions.Like(p.Description, searchPattern) ||
                    EF.Functions.Like(p.Origin, searchPattern) ||
                    EF.Functions.Like(p.Material, searchPattern));
            }

            if (categoryId is > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (gender.HasValue)
            {
                query = query.Where(p => p.Gender == gender.Value);
            }

            if (minPrice is >= 0)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice is >= 0)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (inStockOnly)
            {
                query = query.Where(p => p.StockQuantity > 0);
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedProductsResponseDto
            {
                Items = products.Select(MapToProductResponseDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return MapToProductResponseDto(product);
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto, int? currentUserId)
        {
            var imageUrl = await SaveProductImageAsync(dto.Image);

            var newProduct = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Origin = dto.Origin,
                SizeRange = dto.SizeRange,
                Material = dto.Material,
                Gender = dto.Gender,
                ImageUrl = imageUrl,
                StockQuantity = dto.StockQuantity,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                IsActive = true, 
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(newProduct.Id) ?? throw new Exception("Failed to retrieve created product");
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(ProductUpdateDto dto, int? currentUserId)
        {
            var existingProduct = await _context.Products.FindAsync(dto.Id);

            if (existingProduct == null) return null;

            if (dto.Image is not null)
            {
                existingProduct.ImageUrl = await SaveProductImageAsync(dto.Image);
            }

            existingProduct.Name = dto.Name;
            existingProduct.Price = dto.Price;
            existingProduct.Origin = dto.Origin;
            existingProduct.SizeRange = dto.SizeRange;
            existingProduct.Material = dto.Material;
            existingProduct.Gender = dto.Gender;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.Description = dto.Description;
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.IsActive = dto.IsActive;
            existingProduct.UpdatedByUserId = currentUserId;
            
            // Removed the UpdatedAt line from here

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(existingProduct.Id);
        }

        public async Task<bool> DeleteProductAsync(int id, int? currentUserId)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedByUserId = currentUserId;
            
            // Removed the UpdatedAt line from here
            
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }

        private static ProductResponseDto MapToProductResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Origin = product.Origin,
                SizeRange = product.SizeRange,
                Material = product.Material,
                Gender = product.Gender,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                CategoryDescription = product.Category.Description,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }

        private async Task<string> SaveProductImageAsync(IFormFile image)
        {
            ValidateProductImage(image);

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploadsPath = Path.Combine(webRootPath, "uploads", "products");

            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, fileName);
            await using var stream = new FileStream(filePath, FileMode.CreateNew);
            await image.CopyToAsync(stream);

            return $"/uploads/products/{fileName}";
        }

        private static void ValidateProductImage(IFormFile image)
        {
            if (image.Length == 0)
            {
                throw new ArgumentException("Product image cannot be empty.");
            }

            if (image.Length > MaxImageSizeBytes)
            {
                throw new ArgumentException("Product image cannot be larger than 5 MB.");
            }

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
            {
                throw new ArgumentException("Product image must be a JPG, PNG, or WEBP file.");
            }
        }
    }
}
