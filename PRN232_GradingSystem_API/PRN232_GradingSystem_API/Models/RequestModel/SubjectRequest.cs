using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class SubjectFilterRequest
    {
        public int? Subjectid { get; set; }
        public string? Subjectname { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SubjectRequest
    {
        [Required(ErrorMessage = "Subject name is required.")]
        [StringLength(100, ErrorMessage = "Subject name cannot exceed 100 characters.")]
        public string? SubjectName { get; set; }
    }
}