using B2B_Proje.Business.DTOs.CategoryDTOs;

namespace B2B_Proje.Business.Services.CategoryServices
{
    public enum CategoryDeleteResult
    {
        Deleted,
        NotFound,
        HasProducts
    }

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<bool> CategoryNameExistsAsync(string name, int? excludingCategoryId = null);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task<CategoryDeleteResult> DeleteCategoryAsync(int id);
    }
}
