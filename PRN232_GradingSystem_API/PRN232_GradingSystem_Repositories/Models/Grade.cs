using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("grade")]
public partial class Grade
{
    [Key]
    [Column("grade_id")]
    public int GradeId { get; set; }

    [Column("submission_id")]
    public int? SubmissionId { get; set; }

    [Column("q1")]
    [Precision(5, 2)]
    public decimal? Q1 { get; set; }

    [Column("q2")]
    [Precision(5, 2)]
    public decimal? Q2 { get; set; }

    [Column("q3")]
    [Precision(5, 2)]
    public decimal? Q3 { get; set; }

    [Column("q4")]
    [Precision(5, 2)]
    public decimal? Q4 { get; set; }

    [Column("q5")]
    [Precision(5, 2)]
    public decimal? Q5 { get; set; }

    [Column("q6")]
    [Precision(5, 2)]
    public decimal? Q6 { get; set; }

    [Column("total_score")]
    [Precision(5, 2)]
    public decimal? TotalScore { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("marker_id")]
    public int? MarkerId { get; set; }

    [Column("grade_count")]
    public int? GradeCount { get; set; }

    [Column("plagiarism_score")]
    [Precision(5, 2)]
    public decimal? PlagiarismScore { get; set; }

    [InverseProperty("Grade")]
    public virtual ICollection<GradeDetail> GradeDetails { get; set; } = new List<GradeDetail>();

    [ForeignKey("MarkerId")]
    [InverseProperty("Grades")]
    public virtual AppUser? Marker { get; set; }

    [ForeignKey("SubmissionId")]
    [InverseProperty("Grades")]
    public virtual Submission? Submission { get; set; }
}
