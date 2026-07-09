using System.ComponentModel.DataAnnotations;

namespace B2B_Proje.Business.DTOs.CategoryDTOs
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Category description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
    }

    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Category description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
    }
}
