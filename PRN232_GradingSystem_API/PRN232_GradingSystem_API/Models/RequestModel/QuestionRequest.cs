using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class QuestionFilterRequest
    {
        public int? Questionid { get; set; }
        public int? Examid { get; set; }
        public string? Qcode { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class QuestionRequest
    {
        [Required(ErrorMessage = "ExamId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ExamId must be a positive number.")]
        public int? Examid { get; set; }

        [Required(ErrorMessage = "QCode is required.")]
        [StringLength(10, ErrorMessage = "QCode cannot exceed 10 characters.")]
        public string Qcode { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "MaxScore is required.")]
        [Range(typeof(decimal), "0", "999999", ErrorMessage = "MaxScore must be >= 0.")]
        public decimal? Maxscore { get; set; }
    }

    public class QuestionUpdateRequest
    {
        [Required(ErrorMessage = "QCode is required.")]
        [StringLength(10, ErrorMessage = "QCode cannot exceed 10 characters.")]
        public string Qcode { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "MaxScore is required.")]
        [Range(typeof(decimal), "0", "999999", ErrorMessage = "MaxScore must be >= 0.")]
        public decimal? Maxscore { get; set; }
    }
}