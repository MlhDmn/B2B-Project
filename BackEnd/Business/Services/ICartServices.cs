using B2B_Proje.Business.DTOs.CartDTOs;

namespace B2B_Proje.Business.Services.CartServices
{
    public enum CartOperationStatus
    {
        Success,
        CartItemNotFound,
        ProductNotFound,
        ProductInactive,
        InsufficientStock
    }

    public class CartOperationResult
    {
        public CartOperationStatus Status { get; set; }
        public CartResponseDto? Cart { get; set; }
        public int? AvailableStock { get; set; }
    }

    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int userId);
        Task<CartOperationResult> AddItemAsync(int userId, CartItemCreateDto dto);
        Task<CartOperationResult> UpdateItemAsync(int userId, int cartItemId, CartItemUpdateDto dto);
        Task<CartOperationResult> RemoveItemAsync(int userId, int cartItemId);
        Task<CartResponseDto> ClearCartAsync(int userId);
    }
}
