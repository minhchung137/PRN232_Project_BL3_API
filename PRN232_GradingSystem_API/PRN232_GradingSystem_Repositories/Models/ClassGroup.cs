using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRN232_GradingSystem_Repositories.Models;

[Table("class_group")]
public partial class ClassGroup
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Column("group_name")]
    [StringLength(50)]
    public string GroupName { get; set; } = null!;

    [Column("semester_id")]
    public int? SemesterId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("ClassGroupCreatedByNavigations")]
    public virtual AppUser? CreatedByNavigation { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<GroupStudent> GroupStudents { get; set; } = new List<GroupStudent>();

    [ForeignKey("SemesterId")]
    [InverseProperty("ClassGroups")]
    public virtual Semester? Semester { get; set; }

    [ForeignKey("UpdatedBy")]
    [InverseProperty("ClassGroupUpdatedByNavigations")]
    public virtual AppUser? UpdatedByNavigation { get; set; }
}
