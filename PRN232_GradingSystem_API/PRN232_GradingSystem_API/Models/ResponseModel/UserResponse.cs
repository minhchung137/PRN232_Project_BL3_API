using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Optional navigation properties
        public IReadOnlyList<Grade> Grades { get; set; }
        public IReadOnlyList<Group> GroupCreateByNavigations { get; set; }
        public IReadOnlyList<Group> GroupUpdateByNavigations { get; set; }
        public IReadOnlyList<Semester> SemesterCreatebyNavigations { get; set; }
        public IReadOnlyList<Semester> SemesterUpdatebyNavigations { get; set; }
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long AccessTokenExpiresAtUtc { get; set; }
        public long RefreshTokenExpiresAtUtc { get; set; }
    }
}