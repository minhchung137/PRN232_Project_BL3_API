using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class UserRequest
    {
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class RegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [DefaultValue("chung@gmail.com")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [DefaultValue("Abc@123")]
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [DefaultValue("chung@gmail.com")]
        public string UsernameOrEmail { get; set; }

        [Required]
        [DefaultValue("Abc@123")]
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
