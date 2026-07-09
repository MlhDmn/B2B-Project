using B2B_Proje.Business.DTOs;
using B2B_Proje.Business.DTOs.CategoryDTOs;
using B2B_Proje.Business.Services.CategoryServices;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<CategoryResponseDto>>> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(ApiResponseDto<CategoryResponseDto>.Failure(
                    "CategoryNotFound",
                    $"Category with ID {id} was not found."));
            }

            return Ok(ApiResponseDto<CategoryResponseDto>.Success(
                category,
                "Category retrieved successfully."));
        }

        [Authorize(Policy = "CanManageCategories")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<CategoryResponseDto>>> Create(CategoryCreateDto createDto)
        {
            if (await _categoryService.CategoryNameExistsAsync(createDto.Name))
            {
                return Conflict(ApiResponseDto<CategoryResponseDto>.Failure(
                    "CategoryAlreadyExists",
                    "A category with this name already exists."));
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(createDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdCategory.Id },
                ApiResponseDto<CategoryResponseDto>.Success(createdCategory, "Category created successfully."));
        }

        [Authorize(Policy = "CanManageCategories")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<CategoryResponseDto>>> Update(int id, CategoryUpdateDto updateDto)
        {
            if (await _categoryService.CategoryNameExistsAsync(updateDto.Name, id))
            {
                return Conflict(ApiResponseDto<CategoryResponseDto>.Failure(
                    "CategoryAlreadyExists",
                    "A category with this name already exists."));
            }

            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateDto);

            if (updatedCategory == null)
            {
                return NotFound(ApiResponseDto<CategoryResponseDto>.Failure(
                    "CategoryNotFound",
                    $"Category with ID {id} was not found."));
            }

            return Ok(ApiResponseDto<CategoryResponseDto>.Success(
                updatedCategory,
                "Category updated successfully."));
        }

        [Authorize(Policy = "CanManageCategories")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return result switch
            {
                CategoryDeleteResult.Deleted => Ok(ApiResponseDto<object>.Success(null, "Category deleted successfully.")),
                CategoryDeleteResult.HasProducts => Conflict(ApiResponseDto<object>.Failure(
                    "CategoryHasProducts",
                    "This category cannot be deleted while active products are assigned to it.")),
                _ => NotFound(ApiResponseDto<object>.Failure(
                    "CategoryNotFound",
                    $"Category with ID {id} was not found."))
            };
        }
    }
}
