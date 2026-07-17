using B2B_Proje.Business.DTOs.CartDTOs;
using B2B_Proje.DataAccess.Context;
using B2B_Proje.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2B_Proje.Business.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartResponseDto> GetCartAsync(int userId)
        {
            var cart = await GetCartForResponseAsync(userId);
            return MapToCartResponseDto(cart);
        }

        public async Task<CartOperationResult> AddItemAsync(int userId, CartItemCreateDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(item => item.Id == dto.ProductId);

            if (product == null)
            {
                return new CartOperationResult { Status = CartOperationStatus.ProductNotFound };
            }

            if (!product.IsActive)
            {
                return new CartOperationResult { Status = CartOperationStatus.ProductInactive };
            }

            var cart = await GetOrCreateCartAsync(userId);
            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == dto.ProductId);
            var requestedQuantity = dto.Quantity + (existingItem?.Quantity ?? 0);

            if (requestedQuantity > product.StockQuantity)
            {
                return new CartOperationResult
                {
                    Status = CartOperationStatus.InsufficientStock,
                    AvailableStock = product.StockQuantity
                };
            }

            if (existingItem == null)
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Product = product,
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = userId
                });
            }
            else
            {
                existingItem.Quantity = requestedQuantity;
                existingItem.UpdatedByUserId = userId;
            }

            await _context.SaveChangesAsync();

            return new CartOperationResult
            {
                Status = CartOperationStatus.Success,
                Cart = await GetCartAsync(userId)
            };
        }

        public async Task<CartOperationResult> UpdateItemAsync(int userId, int cartItemId, CartItemUpdateDto dto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(item => item.Id == cartItemId);

            if (cartItem == null)
            {
                return new CartOperationResult { Status = CartOperationStatus.CartItemNotFound };
            }

            if (!cartItem.Product.IsActive)
            {
                return new CartOperationResult { Status = CartOperationStatus.ProductInactive };
            }

            if (dto.Quantity > cartItem.Product.StockQuantity)
            {
                return new CartOperationResult
                {
                    Status = CartOperationStatus.InsufficientStock,
                    AvailableStock = cartItem.Product.StockQuantity
                };
            }

            cartItem.Quantity = dto.Quantity;
            cartItem.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return new CartOperationResult
            {
                Status = CartOperationStatus.Success,
                Cart = await GetCartAsync(userId)
            };
        }

        public async Task<CartOperationResult> RemoveItemAsync(int userId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(item => item.Id == cartItemId);

            if (cartItem == null)
            {
                return new CartOperationResult { Status = CartOperationStatus.CartItemNotFound };
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return new CartOperationResult
            {
                Status = CartOperationStatus.Success,
                Cart = await GetCartAsync(userId)
            };
        }

        public async Task<CartResponseDto> ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            if (cart.CartItems.Count > 0)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }

            return await GetCartAsync(userId);
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(item => item.CartItems)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.UserId == userId);

            if (cart != null)
            {
                return cart;
            }

            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return await _context.Carts
                .Include(item => item.CartItems)
                    .ThenInclude(item => item.Product)
                .FirstAsync(item => item.Id == cart.Id);
        }

        private async Task<Cart> GetCartForResponseAsync(int userId)
        {
            await GetOrCreateCartAsync(userId);

            return await _context.Carts
                .AsNoTracking()
                .Include(item => item.CartItems)
                    .ThenInclude(item => item.Product)
                .FirstAsync(item => item.UserId == userId);
        }

        private static CartResponseDto MapToCartResponseDto(Cart cart)
        {
            var items = cart.CartItems
                .OrderBy(item => item.Id)
                .Select(item => new CartItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ImageUrl = item.Product.ImageUrl,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity,
                    LineTotal = item.Product.Price * item.Quantity,
                    StockQuantity = item.Product.StockQuantity
                })
                .ToList();

            return new CartResponseDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = items,
                TotalQuantity = items.Sum(item => item.Quantity),
                TotalPrice = items.Sum(item => item.LineTotal)
            };
        }
    }
}
