using B2B_Proje.Business.DTOs;
using B2B_Proje.Business.DTOs.CategoryDTOs;
using B2B_Proje.Business.Services.CategoryServices;
using Microsoft.AspNetCore.Mvc;

namespace B2B_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<CategoryResponseDto>>>> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            return Ok(ApiResponseDto<IEnumerable<CategoryResponseDto>>.Success(
                categories,
                "Categories retrieved successfully."));
        }
    }
}
