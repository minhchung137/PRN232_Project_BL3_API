using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("submission")]
public partial class Submission
{
    [Key]
    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [Column("exam_id")]
    public int? ExamId { get; set; }

    [Column("student_id")]
    public int? StudentId { get; set; }

    [Column("solution")]
    public string? Solution { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("file_url")]
    public string? FileUrl { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("Submissions")]
    public virtual Exam? Exam { get; set; }

    [InverseProperty("Submission")]
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [ForeignKey("StudentId")]
    [InverseProperty("Submissions")]
    public virtual Student? Student { get; set; }
}
