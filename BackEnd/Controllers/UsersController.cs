using B2B_Proje.Business.DTOs;
using B2B_Proje.Business.DTOs.UserDTOs;
using B2B_Proje.DataAccess.Context;
using B2B_Proje.DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B2B_Proje.Controllers
{
    [Authorize(Policy = "CanManageUsers")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetById(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (user == null)
            {
                return NotFound(ApiResponseDto<UserResponseDto>.Failure(
                    "UserNotFound",
                    $"User with ID {id} was not found."));
            }

            return Ok(ApiResponseDto<UserResponseDto>.Success(
                new UserResponseDto
                {
                    Id = user.Id,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Permissions = (int)user.Permissions,
                    IsActive = user.IsActive
                },
                "User retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> Update(int id, UserUpdateDto updateDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == id);

            if (user == null)
            {
                return NotFound(ApiResponseDto<UserResponseDto>.Failure(
                    "UserNotFound",
                    $"User with ID {id} was not found."));
            }

            var normalizedEmail = updateDto.Email.Trim().ToLowerInvariant();
            var emailExists = await _context.Users
                .AnyAsync(item => item.Id != id && item.Email == normalizedEmail);

            if (emailExists)
            {
                return Conflict(ApiResponseDto<UserResponseDto>.Failure(
                    "EmailAlreadyExists",
                    "An account with this email already exists."));
            }

            user.FirstName = updateDto.FirstName.Trim();
            user.LastName = updateDto.LastName.Trim();
            user.Email = normalizedEmail;
            user.Permissions = (UserRole)updateDto.Permissions;
            user.IsActive = updateDto.IsActive;
            user.UpdatedByUserId = GetCurrentUserId();

            await _context.SaveChangesAsync();

            return Ok(ApiResponseDto<UserResponseDto>.Success(
                new UserResponseDto
                {
                    Id = user.Id,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Permissions = (int)user.Permissions,
                    IsActive = user.IsActive
                },
                "User updated successfully."));
        }

        private int? GetCurrentUserId()
        {
            var userIdValue = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdValue, out var userId) ? userId : null;
        }
    }
}
