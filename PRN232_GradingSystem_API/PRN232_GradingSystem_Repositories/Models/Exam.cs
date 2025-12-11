using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("exam")]
public partial class Exam
{
    [Key]
    [Column("exam_id")]
    public int ExamId { get; set; }

    [Column("semester_id")]
    public int? SemesterId { get; set; }

    [Column("subject_id")]
    public int? SubjectId { get; set; }

    [Column("exam_name")]
    [StringLength(255)]
    public string? ExamName { get; set; }

    [Column("exam_date")]
    public DateTime? ExamDate { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("SemesterId")]
    [InverseProperty("Exams")]
    public virtual Semester? Semester { get; set; }

    [ForeignKey("SubjectId")]
    [InverseProperty("Exams")]
    public virtual Subject? Subject { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
