using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[PrimaryKey("GroupId", "StudentId")]
[Table("group_student")]
public partial class GroupStudent
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Key]
    [Column("student_id")]
    public int StudentId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("GroupStudents")]
    public virtual ClassGroup Group { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("GroupStudents")]
    public virtual Student Student { get; set; } = null!;
}
