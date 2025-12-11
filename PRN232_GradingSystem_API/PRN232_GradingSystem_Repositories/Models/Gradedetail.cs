using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("grade_detail")]
public partial class GradeDetail
{
    [Key]
    [Column("grade_detail_id")]
    public int GradeDetailId { get; set; }

    [Column("grade_id")]
    public int? GradeId { get; set; }

    [Column("q_code")]
    [StringLength(10)]
    public string? QCode { get; set; }

    [Column("sub_code")]
    [StringLength(50)]
    public string? SubCode { get; set; }

    [Column("point")]
    [Precision(5, 2)]
    public decimal? Point { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("GradeId")]
    [InverseProperty("GradeDetails")]
    public virtual Grade? Grade { get; set; }
}
