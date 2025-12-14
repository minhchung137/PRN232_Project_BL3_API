using System;
using System.Collections.Generic;

namespace PRN232_GradingSystem_Repositories.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int? ExamId { get; set; }

    public string? QCode { get; set; }

    public string? Description { get; set; }

    public decimal? MaxScore { get; set; }

    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();

    public virtual Exam? Exam { get; set; }
}
