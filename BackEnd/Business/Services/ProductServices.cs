using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using B2B_Proje.DataAccess.Context;
using B2B_Proje.DataAccess.Entities; 
using B2B_Proje.Business.DTOs.ProductDTOs;

namespace B2B_Proje.Business.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive) 
                .ToListAsync();

            return products.Select(MapToProductResponseDto);
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return MapToProductResponseDto(product);
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Origin = dto.Origin,
                SizeRange = dto.SizeRange,
                Material = dto.Material,
                Gender = dto.Gender,
                ImageUrl = dto.ImageUrl,
                StockQuantity = dto.StockQuantity,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                IsActive = true, 
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(newProduct.Id) ?? throw new Exception("Failed to retrieve created product");
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(ProductUpdateDto dto)
        {
            var existingProduct = await _context.Products.FindAsync(dto.Id);

            if (existingProduct == null) return null;

            existingProduct.Name = dto.Name;
            existingProduct.Price = dto.Price;
            existingProduct.Origin = dto.Origin;
            existingProduct.SizeRange = dto.SizeRange;
            existingProduct.Material = dto.Material;
            existingProduct.Gender = dto.Gender;
            existingProduct.ImageUrl = dto.ImageUrl;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.Description = dto.Description;
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.IsActive = dto.IsActive;
            
            // Removed the UpdatedAt line from here

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(existingProduct.Id);
        }

        public async Task<bool> DeleteProductAsync(ProductDeleteDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);

            if (product == null) return false;

            product.IsActive = false;
            
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
    }
}
