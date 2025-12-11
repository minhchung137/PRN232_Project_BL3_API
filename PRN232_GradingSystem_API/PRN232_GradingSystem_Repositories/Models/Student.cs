using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("student")]
[Index("StudentRoll", Name = "student_student_roll_key", IsUnique = true)]
public partial class Student
{
    [Key]
    [Column("student_id")]
    public int StudentId { get; set; }

    [Column("student_fullname")]
    [StringLength(100)]
    public string? StudentFullname { get; set; }

    [Column("student_roll")]
    [StringLength(20)]
    public string StudentRoll { get; set; } = null!;

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<GroupStudent> GroupStudents { get; set; } = new List<GroupStudent>();

    [InverseProperty("Student")]
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
