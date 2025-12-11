using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("semester")]
[Index("SemesterCode", Name = "semester_semester_code_key", IsUnique = true)]
public partial class Semester
{
    [Key]
    [Column("semester_id")]
    public int SemesterId { get; set; }

    [Column("semester_code")]
    [StringLength(50)]
    public string SemesterCode { get; set; } = null!;

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [InverseProperty("Semester")]
    public virtual ICollection<ClassGroup> ClassGroups { get; set; } = new List<ClassGroup>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("SemesterCreatedByNavigations")]
    public virtual AppUser? CreatedByNavigation { get; set; }

    [InverseProperty("Semester")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Semester")]
    public virtual ICollection<SemesterSubject> SemesterSubjects { get; set; } = new List<SemesterSubject>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("SemesterUpdatedByNavigations")]
    public virtual AppUser? UpdatedByNavigation { get; set; }
}
