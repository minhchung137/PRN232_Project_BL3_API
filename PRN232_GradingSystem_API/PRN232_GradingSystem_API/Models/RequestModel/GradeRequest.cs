using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class GradeFilterRequest
    {
        public int? Gradeid { get; set; }
        public int? Submissionid { get; set; }
        public decimal? Q1 { get; set; }

        public decimal? Q2 { get; set; }

        public decimal? Q3 { get; set; }

        public decimal? Q4 { get; set; }

        public decimal? Q5 { get; set; }

        public decimal? Q6 { get; set; }

        public decimal? Totalscore { get; set; }
        public string? Status { get; set; }

        public DateTime? Createat { get; set; }

        public DateTime? Updateat { get; set; }
        public int? Marker { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public partial class GradeRequest
    {
        [Required(ErrorMessage = "Submission ID is required.")]
        public int? Submissionid { get; set; }

        [Range(0, 1, ErrorMessage = "Q1 must be between 0 and 1.")]
        public decimal? Q1 { get; set; }

        [Range(0, 1.5, ErrorMessage = "Q2 must be between 0 and 1.5.")]
        public decimal? Q2 { get; set; }

        [Range(0, 2.5, ErrorMessage = "Q3 must be between 0 and 2.5.")]
        public decimal? Q3 { get; set; }

        [Range(0, 2, ErrorMessage = "Q4 must be between 0 and 2.")]
        public decimal? Q4 { get; set; }

        [Range(0, 1.5, ErrorMessage = "Q5 must be between 0 and 1.5.")]
        public decimal? Q5 { get; set; }

        [Range(0, 1.5, ErrorMessage = "Q6 must be between 0 and 1.5.")]
        public decimal? Q6 { get; set; }

        [Required(ErrorMessage = "Marker is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "MarkerID must be positive number")]
        public int? Marker { get; set; }
    }

    public class GradeWithDetailsRequest
    {
        [Required]
        public int? Submissionid { get; set; }

        [Required(ErrorMessage = "Marker (email) is required.")]
        [EmailAddress(ErrorMessage = "Marker must be a valid email address.")]
        public string? Marker { get; set; }
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string? Comment { get; set; }

        // Chi tiết từng câu
        [Required]
        [MinLength(1, ErrorMessage = "At least one gradedetail must be provided.")]
        public List<GradedetailCreateRequest> Gradedetails { get; set; } = new();
    }
}
