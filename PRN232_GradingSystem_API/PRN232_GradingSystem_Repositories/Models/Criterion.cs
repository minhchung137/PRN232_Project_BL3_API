using System;
using System.Collections.Generic;

namespace PRN232_GradingSystem_Repositories.Models;

public partial class Criterion
{
    public int CriteriaId { get; set; }

    public int? QuestionId { get; set; }

    public string Content { get; set; } = null!;

    public decimal Weight { get; set; }

    public bool? IsManual { get; set; }

    public int OrderIndex { get; set; }

    public virtual ICollection<GradeDetail> GradeDetails { get; set; } = new List<GradeDetail>();

    public virtual Question? Question { get; set; }
}
