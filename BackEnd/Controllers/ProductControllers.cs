using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound($"Product with ID {id} not found.");
            
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); 

            var createdProduct = await _productService.CreateProductAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponseDto>> Update(int id, [FromBody] ProductUpdateDto updateDto)
        {
            if (id != updateDto.Id) return BadRequest("The ID in the URL does not match the payload ID.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedProduct = await _productService.UpdateProductAsync(updateDto);
            
            if (updatedProduct == null) return NotFound($"Product with ID {id} not found.");

            return Ok(updatedProduct);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromBody] ProductDeleteDto deleteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _productService.DeleteProductAsync(deleteDto);
            
            if (!success) return NotFound($"Product with ID {deleteDto.Id} not found.");

            return NoContent(); 
        }
    }
}