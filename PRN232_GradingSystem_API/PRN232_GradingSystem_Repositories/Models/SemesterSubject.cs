using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[PrimaryKey("SemesterId", "SubjectId")]
[Table("semester_subject")]
public partial class SemesterSubject
{
    [Key]
    [Column("semester_id")]
    public int SemesterId { get; set; }

    [Key]
    [Column("subject_id")]
    public int SubjectId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("SemesterId")]
    [InverseProperty("SemesterSubjects")]
    public virtual Semester Semester { get; set; } = null!;

    [ForeignKey("SubjectId")]
    [InverseProperty("SemesterSubjects")]
    public virtual Subject Subject { get; set; } = null!;
}
