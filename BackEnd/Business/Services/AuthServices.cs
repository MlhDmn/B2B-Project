using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using B2B_Proje.Business.DTOs.AuthDTOs;
using B2B_Proje.DataAccess.Context;
using B2B_Proje.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace B2B_Proje.Business.Services.AuthServices
{
    public interface ITokenService
    {
        AuthResponseDto CreateToken(User user);
    }

    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthResponseDto CreateToken(User user)
        {
            var expirationMinutes = _configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 60;
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes);
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("permissions", ((int)user.Permissions).ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new AuthResponseDto(
                new JwtSecurityTokenHandler().WriteToken(token),
                expiresAtUtc,
                new AuthUserDto(
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName));
        }
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (await _context.Users.AnyAsync(user => user.Email == normalizedEmail))
                return null;

            var user = new User
            {
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _tokenService.CreateToken(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            if (!user.IsActive)
                return null;

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _tokenService.CreateToken(user);
        }
    }
}
