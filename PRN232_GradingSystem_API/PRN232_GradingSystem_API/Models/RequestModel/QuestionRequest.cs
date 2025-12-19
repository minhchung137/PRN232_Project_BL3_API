using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class QuestionFilterRequest
    {
        public int? Questionid { get; set; } = 1;
        public int? Examid { get; set; } = 1;
        public string? Qcode { get; set; } = "Q1";

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class QuestionRequest
    {
        [DefaultValue(1)]
        [Required(ErrorMessage = "ExamId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ExamId must be a positive number.")]
        public int? Examid { get; set; }

        [Required(ErrorMessage = "QCode is required.")]
        [DefaultValue("Q1")]
        [StringLength(10, ErrorMessage = "QCode cannot exceed 10 characters.")]
        public string Qcode { get; set; }

        [DefaultValue("Creat new object successfully")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "MaxScore is required.")]
        [DefaultValue(1.0)]
        [Range(typeof(decimal), "0", "10", ErrorMessage = "MaxScore must be >= 0.")]
        public decimal? Maxscore { get; set; }
    }

    public class QuestionUpdateRequest
    {
        [Required(ErrorMessage = "QCode is required.")]
        [DefaultValue("Q1")]
        [StringLength(10, ErrorMessage = "QCode cannot exceed 10 characters.")]
        public string Qcode { get; set; }
        [DefaultValue("Creat new object successfully")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "MaxScore is required.")]
        [DefaultValue(1.0)]
        [Range(typeof(decimal), "0", "10", ErrorMessage = "MaxScore must be >= 0.")]
        public decimal? Maxscore { get; set; }
    }
}