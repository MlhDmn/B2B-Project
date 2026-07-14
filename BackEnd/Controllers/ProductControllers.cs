using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using B2B_Proje.Business.DTOs;
using B2B_Proje.Business.DTOs.ProductDTOs;
using B2B_Proje.Business.Services.ProductServices;
using B2B_Proje.DataAccess.Enums;

namespace B2B_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<PagedProductsResponseDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] ProductGender? gender = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool inStockOnly = false,
            [FromQuery] ProductSortOption sortBy = ProductSortOption.NameAsc)
        {
            var products = await _productService.GetAllProductsAsync(
                pageNumber,
                pageSize,
                searchTerm,
                categoryId,
                gender,
                minPrice,
                maxPrice,
                inStockOnly,
                sortBy);

            return Ok(ApiResponseDto<PagedProductsResponseDto>.Success(
                products,
                "Products retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponseDto<ProductResponseDto>.Failure(
                    "ProductNotFound",
                    $"Product with ID {id} not found."));
            }
            
            return Ok(ApiResponseDto<ProductResponseDto>.Success(
                product,
                "Product retrieved successfully."));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> Create([FromForm] ProductCreateDto createDto)
        {
            var currentUserId = GetCurrentUserId();
            ProductResponseDto createdProduct;

            try
            {
                createdProduct = await _productService.CreateProductAsync(createDto, currentUserId);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponseDto<ProductResponseDto>.Failure(
                    "InvalidProductImage",
                    exception.Message));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                ApiResponseDto<ProductResponseDto>.Success(createdProduct, "Product created successfully."));
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> Update(int id, [FromForm] ProductUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(ApiResponseDto<ProductResponseDto>.Failure(
                    "IdMismatch",
                    "The ID in the URL does not match the payload ID."));
            }

            var currentUserId = GetCurrentUserId();
            ProductResponseDto? updatedProduct;

            try
            {
                updatedProduct = await _productService.UpdateProductAsync(updateDto, currentUserId);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponseDto<ProductResponseDto>.Failure(
                    "InvalidProductImage",
                    exception.Message));
            }
            
            if (updatedProduct == null)
            {
                return NotFound(ApiResponseDto<ProductResponseDto>.Failure(
                    "ProductNotFound",
                    $"Product with ID {id} not found."));
            }

            return Ok(ApiResponseDto<ProductResponseDto>.Success(
                updatedProduct,
                "Product updated successfully."));
        }

        private int? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdValue, out var userId) ? userId : null;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            var success = await _productService.DeleteProductAsync(id, currentUserId);
            
            if (!success)
            {
                return NotFound(ApiResponseDto<object>.Failure(
                    "ProductNotFound",
                    $"Product with ID {id} not found."));
            }

            return Ok(ApiResponseDto<object>.Success(null, "Product deleted successfully.")); 
        }
    }
}
