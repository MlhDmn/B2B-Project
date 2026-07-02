using B2B_Proje.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace B2B_Proje.Business.DTOs.ProductDTOs
{

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Origin { get; set; }
        public string SizeRange { get; set; }
        public string Material { get; set; }
        public ProductGender Gender { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [MaxLength(50)] 
        [Required(ErrorMessage = "Origin is required.")]
        public required string Origin { get; set; }

        [MaxLength(50)] 
        [Required(ErrorMessage = "Size range is required.")]
        public required string SizeRange { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Material name is required.")]
        public required string Material { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public required ProductGender Gender { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        public required string ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "You must select a category.")]
        public int CategoryId { get; set; }
    }

    public class ProductUpdateDto
    {
        [Required] 
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100)] 
        public required string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [MaxLength(50)] 
        [Required(ErrorMessage = "Origin is required.")]
        public required string Origin { get; set; }

        [MaxLength(50)] 
        [Required(ErrorMessage = "Size range is required.")]
        public required string SizeRange { get; set; }

        [MaxLength(50)] 
        [Required(ErrorMessage = "Material name is required.")]
        public required string Material { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public required ProductGender Gender { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        public required string ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")] 
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "You must select a category.")]
        public int CategoryId { get; set; }
        
        public bool IsActive { get; set; }
    }
    
    public class ProductDeleteDto
    {
        [Required(ErrorMessage = "Product ID is required for deletion.")]
        public int Id { get; set; }
    }
}