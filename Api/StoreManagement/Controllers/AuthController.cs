using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Data;
using StoreManagement.Models.Dtos;
using StoreManagement.Services;

namespace StoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StoreDbContext dbContext;
        private readonly ITokenService tokenService;

        private readonly PasswordHasher<Models.Entities.User> passwordHasher = new();

        public AuthController(StoreDbContext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var verification = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var (token, expiryDate) = tokenService.GenerateToken(user);


            return Ok(new LoginResponseDto
            {
                UserId = user.UserId,
                UserFullName = user.UserFullName,
                UserName = user.UserName,
                UserType = (int)user.UserType,
                UserTypeName = user.UserType.ToString(),
                Token = token,
                ExpiresAt = expiryDate
            });
        }
    }
}
