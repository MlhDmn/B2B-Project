using System.ComponentModel.DataAnnotations;

namespace B2B_Proje.Business.DTOs.CartDTOs
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public IEnumerable<CartItemResponseDto> Items { get; set; } = [];
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CartItemCreateDto
    {
        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; } = 1;
    }

    public class CartItemUpdateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}
