using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("app_user")]
[Index("Email", Name = "app_user_email_key", IsUnique = true)]
public partial class AppUser
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password")]
    public byte[]? Password { get; set; }

    [Column("salt")]
    public byte[]? Salt { get; set; }

    [Column("role")]
    [StringLength(50)]
    public string? Role { get; set; }

    [Column("refresh_token")]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry_time")]
    public DateTime? RefreshTokenExpiryTime { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<ClassGroup> ClassGroupCreatedByNavigations { get; set; } = new List<ClassGroup>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<ClassGroup> ClassGroupUpdatedByNavigations { get; set; } = new List<ClassGroup>();

    [InverseProperty("Marker")]
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Semester> SemesterCreatedByNavigations { get; set; } = new List<Semester>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Semester> SemesterUpdatedByNavigations { get; set; } = new List<Semester>();
}
