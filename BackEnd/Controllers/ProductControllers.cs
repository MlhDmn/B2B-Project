using Microsoft.AspNetCore.Mvc;
using B2B_Proje.Business.DTOs;
using B2B_Proje.Business.DTOs.ProductDTOs;
using B2B_Proje.Business.Services.ProductServices;

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
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ProductResponseDto>>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(ApiResponseDto<IEnumerable<ProductResponseDto>>.Success(
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
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> Create([FromBody] ProductCreateDto createDto)
        {
            var createdProduct = await _productService.CreateProductAsync(createDto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                ApiResponseDto<ProductResponseDto>.Success(createdProduct, "Product created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> Update(int id, [FromBody] ProductUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(ApiResponseDto<ProductResponseDto>.Failure(
                    "IdMismatch",
                    "The ID in the URL does not match the payload ID."));
            }

            var updatedProduct = await _productService.UpdateProductAsync(updateDto);
            
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

        [HttpDelete]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete([FromBody] ProductDeleteDto deleteDto)
        {
            var success = await _productService.DeleteProductAsync(deleteDto);
            
            if (!success)
            {
                return NotFound(ApiResponseDto<object>.Failure(
                    "ProductNotFound",
                    $"Product with ID {deleteDto.Id} not found."));
            }

            return Ok(ApiResponseDto<object>.Success(null, "Product deleted successfully.")); 
        }
    }
}
