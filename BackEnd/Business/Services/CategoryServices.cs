using B2B_Proje.Business.DTOs.CategoryDTOs;
using B2B_Proje.DataAccess.Context;
using B2B_Proje.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2B_Proje.Business.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(category => category.IsActive)
                .OrderBy(category => category.Name)
                .ToListAsync();

            return categories.Select(MapToCategoryResponseDto);
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(category => category.Id == id && category.IsActive);

            return category == null ? null : MapToCategoryResponseDto(category);
        }

        public async Task<bool> CategoryNameExistsAsync(string name, int? excludingCategoryId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Categories.AnyAsync(category =>
                category.IsActive &&
                category.Name.ToLower() == normalizedName &&
                (!excludingCategoryId.HasValue || category.Id != excludingCategoryId.Value));
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToCategoryResponseDto(category);
        }

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(category => category.Id == id && category.IsActive);

            if (category == null)
            {
                return null;
            }

            category.Name = dto.Name.Trim();
            category.Description = dto.Description.Trim();

            await _context.SaveChangesAsync();

            return MapToCategoryResponseDto(category);
        }

        public async Task<CategoryDeleteResult> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(category => category.Id == id && category.IsActive);

            if (category == null)
            {
                return CategoryDeleteResult.NotFound;
            }

            var hasActiveProducts = await _context.Products
                .AnyAsync(product => product.CategoryId == id && product.IsActive);

            if (hasActiveProducts)
            {
                return CategoryDeleteResult.HasProducts;
            }

            category.IsActive = false;
            await _context.SaveChangesAsync();

            return CategoryDeleteResult.Deleted;
        }

        private static CategoryResponseDto MapToCategoryResponseDto(Category category)
        {
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
