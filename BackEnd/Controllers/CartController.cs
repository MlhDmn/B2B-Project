using System.Security.Claims;
using B2B_Proje.Business.DTOs;
using B2B_Proje.Business.DTOs.CartDTOs;
using B2B_Proje.Business.Services.CartServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2B_Proje.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<CartResponseDto>>> Get()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponseDto<CartResponseDto>.Failure(
                    "Unauthorized",
                    "User id could not be resolved from the token."));
            }

            var cart = await _cartService.GetCartAsync(userId.Value);

            return Ok(ApiResponseDto<CartResponseDto>.Success(
                cart,
                "Cart retrieved successfully."));
        }

        [HttpPost("items")]
        public async Task<ActionResult<ApiResponseDto<CartResponseDto>>> AddItem(CartItemCreateDto createDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponseDto<CartResponseDto>.Failure(
                    "Unauthorized",
                    "User id could not be resolved from the token."));
            }

            var result = await _cartService.AddItemAsync(userId.Value, createDto);
            return ToCartActionResult(result, "Item added to cart successfully.");
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<ActionResult<ApiResponseDto<CartResponseDto>>> UpdateItem(
            int cartItemId,
            CartItemUpdateDto updateDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponseDto<CartResponseDto>.Failure(
                    "Unauthorized",
                    "User id could not be resolved from the token."));
            }

            var result = await _cartService.UpdateItemAsync(userId.Value, cartItemId, updateDto);
            return ToCartActionResult(result, "Cart item updated successfully.");
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<ActionResult<ApiResponseDto<CartResponseDto>>> RemoveItem(int cartItemId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponseDto<CartResponseDto>.Failure(
                    "Unauthorized",
                    "User id could not be resolved from the token."));
            }

            var result = await _cartService.RemoveItemAsync(userId.Value, cartItemId);
            return ToCartActionResult(result, "Item removed from cart successfully.");
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponseDto<CartResponseDto>>> Clear()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponseDto<CartResponseDto>.Failure(
                    "Unauthorized",
                    "User id could not be resolved from the token."));
            }

            var cart = await _cartService.ClearCartAsync(userId.Value);

            return Ok(ApiResponseDto<CartResponseDto>.Success(
                cart,
                "Cart cleared successfully."));
        }

        private ActionResult<ApiResponseDto<CartResponseDto>> ToCartActionResult(
            CartOperationResult result,
            string successMessage)
        {
            return result.Status switch
            {
                CartOperationStatus.Success => Ok(ApiResponseDto<CartResponseDto>.Success(
                    result.Cart,
                    successMessage)),
                CartOperationStatus.CartItemNotFound => NotFound(ApiResponseDto<CartResponseDto>.Failure(
                    "CartItemNotFound",
                    "Cart item was not found.")),
                CartOperationStatus.ProductNotFound => NotFound(ApiResponseDto<CartResponseDto>.Failure(
                    "ProductNotFound",
                    "Product was not found.")),
                CartOperationStatus.ProductInactive => Conflict(ApiResponseDto<CartResponseDto>.Failure(
                    "ProductInactive",
                    "This product is no longer available.")),
                CartOperationStatus.InsufficientStock => Conflict(ApiResponseDto<CartResponseDto>.Failure(
                    "InsufficientStock",
                    $"Only {result.AvailableStock ?? 0} item(s) are available in stock.")),
                _ => BadRequest(ApiResponseDto<CartResponseDto>.Failure(
                    "CartOperationFailed",
                    "Cart operation failed."))
            };
        }

        private int? GetCurrentUserId()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdValue, out var userId) ? userId : null;
        }
    }
}
