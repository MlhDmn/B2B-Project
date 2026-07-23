using System.ComponentModel.DataAnnotations;

namespace B2B_Proje.Business.DTOs.AuthDTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$", ErrorMessage = "Please enter a valid email address with a domain, for example name@example.com.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
    }

    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$", ErrorMessage = "Please enter a valid email address with a domain, for example name@example.com.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public record AuthUserDto(
        int UserId,
        string Email,
        string FirstName,
        string LastName);

    public record AuthResponseDto(
        string Token,
        DateTime ExpiresAtUtc,
        AuthUserDto User);
}
