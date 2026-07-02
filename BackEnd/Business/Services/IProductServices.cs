using B2B_Proje.Business.DTOs.ProductDTOs;

namespace B2B_Proje.Business.Services.ProductServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto);
        Task<ProductResponseDto?> UpdateProductAsync(ProductUpdateDto dto);
        Task<bool> DeleteProductAsync(ProductDeleteDto dto); 
    }
}