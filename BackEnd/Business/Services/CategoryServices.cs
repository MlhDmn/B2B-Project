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
                .OrderBy(category => category.Name)
                .ToListAsync();

            return categories.Select(MapToCategoryResponseDto);
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
