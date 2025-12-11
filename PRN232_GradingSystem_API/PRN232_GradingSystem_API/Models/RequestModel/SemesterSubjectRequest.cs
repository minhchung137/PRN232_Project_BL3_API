using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class SemesterSubjectFilterRequest
    {
        public int? Semesterid { get; set; }
        public int? Subjectid { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class SemesterSubjectRequest
    {
        [Required(ErrorMessage = "Semester ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Semester ID must be greater than 0.")]
        public int Semesterid { get; set; }

        [Required(ErrorMessage = "Subject ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Subject ID must be greater than 0.")]
        public int Subjectid { get; set; }
    }
}