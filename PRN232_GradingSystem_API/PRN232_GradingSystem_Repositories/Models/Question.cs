using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("question")]
public partial class Question
{
    [Key]
    [Column("question_id")]
    public int QuestionId { get; set; }
    [Column("exam_id")]
    public int? ExamId { get; set; }
    [Column("q_code")]
    public string? QCode { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    [Column("max_score", TypeName = "numeric(5,2)")]
    public decimal? MaxScore { get; set; }
    [InverseProperty(nameof(Criterion.Question))]
    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();
    [ForeignKey(nameof(ExamId))]
    public virtual Exam? Exam { get; set; }
}
