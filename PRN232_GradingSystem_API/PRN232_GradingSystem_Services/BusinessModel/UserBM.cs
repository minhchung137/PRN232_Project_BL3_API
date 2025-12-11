using System;
using System.Collections.Generic;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class UserBM
    {
        public int Userid { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Role { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Createat { get; set; }

        public IReadOnlyList<Grade> Grades { get; set; }
        public IReadOnlyList<ClassGroup> GroupCreatebyNavigations { get; set; }
        public IReadOnlyList<ClassGroup> GroupUpdatebyNavigations { get; set; }
        public IReadOnlyList<Semester> SemesterCreatebyNavigations { get; set; }
        public IReadOnlyList<Semester> SemesterUpdatebyNavigations { get; set; }

    }

    public class AuthResultModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
    }
}
