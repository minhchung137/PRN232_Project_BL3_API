using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class ExamFilterRequest
    {
        public int? Examid { get; set; }
        public int? Semesterid { get; set; }
        public int? Subjectid { get; set; }
        public string? Examname { get; set; }
        public DateTime? Examdate { get; set; }
        public DateTime? Createat { get; set; }


        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class ExamRequest
    {
        [Required(ErrorMessage = "SemesterId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be a positive number.")]
        public int? Semesterid { get; set; }

        [Required(ErrorMessage = "SubjectId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SubjectId must be a positive number.")]
        public int? Subjectid { get; set; }

        [Required(ErrorMessage = "Exam name is required.")]
        [StringLength(100, ErrorMessage = "Exam name cannot exceed 100 characters.")]
        public string Examname { get; set; }

        [Required(ErrorMessage = "Exam date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime? Examdate { get; set; }
    }

}
