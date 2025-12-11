using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Services.Services.Implementations;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class GradedetailFilterRequest
    {
        public int? Gradeid { get; set; }
        public string? Qcode { get; set; }
        public string? Subcode { get; set; }
        public decimal? Point { get; set; }
        public string? Note { get; set; }
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    [GradedetailValidation]

    public class GradedetailCreateRequest
    {
        [Required(ErrorMessage = "Gradeid must be provided.")]
        public int? Gradeid { get; set; }

        [Required(ErrorMessage = "Qcode must be provided.")]
        public string Qcode { get; set; }

        [Required(ErrorMessage = "Subcode must be provided.")]
        public string Subcode { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Point cannot be negative.")]
        public decimal? Point { get; set; }

        public string Note { get; set; }
    }


    public class GradedetailUpdateItem
    {
        [Required]
        public int Gradedetailid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Point cannot be negative.")]
        public decimal? Point { get; set; }

        public string Note { get; set; }

    }
    public class GradedetailUpdateRequestList
    {
        [Required]
        public List<GradedetailUpdateItem> Gradedetails { get; set; } = new();
    }

}
