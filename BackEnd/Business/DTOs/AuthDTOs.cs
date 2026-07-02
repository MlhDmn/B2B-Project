using System.ComponentModel.DataAnnotations;

namespace B2B_Proje.Business.DTOs.AuthDTOs
{
    public class RegisterRequestDto
    {
        [Required, EmailAddress, MaxLength(100)]
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
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public record AuthResponseDto(
        string Token,
        DateTime ExpiresAtUtc,
        int UserId,
        string Email,
        string FirstName,
        string LastName);
}
