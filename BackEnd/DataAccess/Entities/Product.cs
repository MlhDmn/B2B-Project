using B2B_Proje.DataAccess.Enums;

namespace B2B_Proje.DataAccess.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } 
        public string Origin { get; set; } = string.Empty;
        public string SizeRange { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public ProductGender Gender { get; set; } = ProductGender.Unisex; 
        public string ImageUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; } 
        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; } 
        public Category Category { get; set; } = null!;
    }
}