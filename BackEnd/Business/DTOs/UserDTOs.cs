using System.ComponentModel.DataAnnotations;

namespace B2B_Proje.Business.DTOs.UserDTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Permissions { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserUpdateDto
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$", ErrorMessage = "Please enter a valid email address with a domain, for example name@example.com.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public int Permissions { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
