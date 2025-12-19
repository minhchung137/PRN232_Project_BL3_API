using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Services.Services.Implementations;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class GradedetailFilterRequest
    {
        public int? Gradeid { get; set; } = 1;
        public string? Qcode { get; set; } = "Q1";
        public string? Subcode { get; set; } = "C1";
        public decimal? Point { get; set; } = 1;
        public string? Note { get; set; } = "Good job";
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    //[GradedetailValidation]

    public class GradedetailCreateRequest
    {
        [Required(ErrorMessage = "Gradeid must be provided.")]
        public int? Gradeid { get; set; } = 1;

        [Required(ErrorMessage = "Qcode must be provided.")]
        public string Qcode { get; set; } = "Q1";

        [Required(ErrorMessage = "Subcode must be provided.")]
        public string Subcode { get; set; } = "C1";

        [Range(0, double.MaxValue, ErrorMessage = "Point cannot be negative.")]
        public decimal? Point { get; set; } = 1;

        public string Note { get; set; } = "Good job";
    }


    public class GradedetailUpdateItem
    {
        [Required]
        public int Gradedetailid { get; set; } = 1;

        [Range(0, double.MaxValue, ErrorMessage = "Point cannot be negative.")]
        public decimal? Point { get; set; } = 1;

        public string Note { get; set; } = "Good job";

    }
    public class GradedetailUpdateRequestList
    {
        [Required]
        public List<GradedetailUpdateItem> Gradedetails { get; set; } = new();
    }

}
