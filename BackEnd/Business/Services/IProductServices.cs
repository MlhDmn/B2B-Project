using B2B_Proje.Business.DTOs.ProductDTOs;

namespace B2B_Proje.Business.Services.ProductServices
{
    public interface IProductService
    {
        Task<PagedProductsResponseDto> GetAllProductsAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto, int? currentUserId);
        Task<ProductResponseDto?> UpdateProductAsync(ProductUpdateDto dto, int? currentUserId);
        Task<bool> DeleteProductAsync(int id, int? currentUserId); 
    }
}
