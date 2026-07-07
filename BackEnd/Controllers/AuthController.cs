using B2B_Proje.Business.DTOs.AuthDTOs;
using B2B_Proje.Business.DTOs;
using B2B_Proje.Business.Services.AuthServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2B_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Register(RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            if (response == null)
            {
                return Conflict(ApiResponseDto<AuthResponseDto>.Failure(
                    "EmailAlreadyExists",
                    "An account with this email already exists."));
            }

            return Ok(ApiResponseDto<AuthResponseDto>.Success(response, "Registration successful."));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Login(LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(ApiResponseDto<AuthResponseDto>.Failure(
                    "InvalidCredentials",
                    "Invalid email or password."));
            }

            return Ok(ApiResponseDto<AuthResponseDto>.Success(response, "Login successful."));
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value));
        }
    }
}
