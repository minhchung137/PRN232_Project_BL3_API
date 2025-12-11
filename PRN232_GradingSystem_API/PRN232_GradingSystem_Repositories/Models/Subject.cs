using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("subject")]
public partial class Subject
{
    [Key]
    [Column("subject_id")]
    public int SubjectId { get; set; }

    [Column("subject_name")]
    [StringLength(100)]
    public string SubjectName { get; set; } = null!;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Subject")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Subject")]
    public virtual ICollection<SemesterSubject> SemesterSubjects { get; set; } = new List<SemesterSubject>();
}
