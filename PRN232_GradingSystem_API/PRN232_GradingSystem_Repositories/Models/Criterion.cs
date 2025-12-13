using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232_GradingSystem_Repositories.Models;
[Table("criterion")]
public partial class Criterion
{
    [Key]
    [Column("criterion_id")]
    public int CriterionId { get; set; }

    [Column("question_id")]
    public int? QuestionId { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("weight", TypeName = "numeric(5,2)")]
    public decimal Weight { get; set; }

    [Column("is_manual")]
    public bool? IsManual { get; set; }

    [Column("order_index")]
    public int OrderIndex { get; set; }
    
    [InverseProperty(nameof(GradeDetail.Criteria))]
    public virtual ICollection<GradeDetail> GradeDetails { get; set; } = new List<GradeDetail>();

    [ForeignKey(nameof(QuestionId))]
    [InverseProperty(nameof(Question.Criteria))]
    public virtual Question? Question { get; set; }
}
