using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232.Lab2.CoffeeStore.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(400, ApiResponse<object>.FailResponse("Invalid request", 400));
            var result = await _userService.RegisterAsync(request.Username, request.Email, request.Password);
            if (!result.Success)
                return StatusCode(result.StatusCode, result);
            var data = result.Data;
            var response = new AuthResponse
            {
                UserId = data.UserId,
                Username = data.Username,
                Email = data.Email,
                Role = data.Role,
                AccessToken = data.AccessToken,
                RefreshToken = data.RefreshToken,
                AccessTokenExpiresAtUtc = new DateTimeOffset(data.AccessTokenExpiresAtUtc).ToUnixTimeSeconds(),
                RefreshTokenExpiresAtUtc = new DateTimeOffset(data.RefreshTokenExpiresAtUtc).ToUnixTimeSeconds()
            };
            return StatusCode(201, ApiResponse<AuthResponse>.SuccessResponse(response, "Registered", 201));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(400, ApiResponse<object>.FailResponse("Invalid request", 400));
            var result = await _userService.LoginAsync(request.UsernameOrEmail, request.Password);
            if (!result.Success)
                return StatusCode(result.StatusCode, result);
            var data = result.Data;
            var response = new AuthResponse
            {
                UserId = data.UserId,
                Username = data.Username,
                Email = data.Email,
                Role = data.Role,
                AccessToken = data.AccessToken,
                RefreshToken = data.RefreshToken,
                AccessTokenExpiresAtUtc = new DateTimeOffset(data.AccessTokenExpiresAtUtc).ToUnixTimeSeconds(),
                RefreshTokenExpiresAtUtc = new DateTimeOffset(data.RefreshTokenExpiresAtUtc).ToUnixTimeSeconds()
            };
            return StatusCode(200, ApiResponse<AuthResponse>.SuccessResponse(response, "Logged in", 200));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(400, ApiResponse<object>.FailResponse("Invalid request", 400));
            var result = await _userService.RefreshTokenAsync(request.RefreshToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result);
            var data = result.Data;
            var response = new AuthResponse
            {
                UserId = data.UserId,
                Username = data.Username,
                Email = data.Email,
                Role = data.Role,
                AccessToken = data.AccessToken,
                RefreshToken = data.RefreshToken,
                AccessTokenExpiresAtUtc = new DateTimeOffset(data.AccessTokenExpiresAtUtc).ToUnixTimeSeconds(),
                RefreshTokenExpiresAtUtc = new DateTimeOffset(data.RefreshTokenExpiresAtUtc).ToUnixTimeSeconds()
            };
            return StatusCode(200, ApiResponse<AuthResponse>.SuccessResponse(response, "Token refreshed", 200));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(400, ApiResponse<object>.FailResponse("Invalid request", 400));
            var result = await _userService.LogoutAsync(request.RefreshToken);
            if (result.StatusCode == 204) return StatusCode(204);
            return StatusCode(result.StatusCode, result);
        }
    }
}
