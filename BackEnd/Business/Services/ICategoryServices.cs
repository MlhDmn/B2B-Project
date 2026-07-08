using B2B_Proje.Business.DTOs.CategoryDTOs;

namespace B2B_Proje.Business.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
    }
}
