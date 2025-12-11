using PRN232_GradingSystem_Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class SubmissionFilterRequest
    {
        public int? Submissionid { get; set; }
        public int? Examid { get; set; }
        public int? Studentid { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class SubmissionRequest
    {
        [Required(ErrorMessage = "Exam ID must be provided.")]
        [Range(1, int.MaxValue, ErrorMessage = "Exam ID must be a positive number.")]
        public int Examid { get; set; }

        [Required(ErrorMessage = "Student ID must be provided.")]
        [Range(1, int.MaxValue, ErrorMessage = "Student ID must be a positive number.")]
        public int Studentid { get; set; }
        [StringLength(5000, ErrorMessage = "Solution cannot exceed 5000 characters.")]
        public string Solution { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }

        [RegularExpression(@"^$|^(http|https)://", ErrorMessage = "File URL must be a valid URL.")]
        public string? Fileurl { get; set; }
    }
}