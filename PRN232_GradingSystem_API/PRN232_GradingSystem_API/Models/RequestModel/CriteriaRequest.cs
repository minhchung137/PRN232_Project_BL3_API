using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class CriteriaFilterRequest
    {
        public int? Criteriaid { get; set; }
        public int? Questionid { get; set; }
        public bool? Ismanual { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CriteriaRequest
    {
        [Required(ErrorMessage = "QuestionId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "QuestionId must be a positive number.")]
        public int? Questionid { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(255, ErrorMessage = "Content cannot exceed 255 characters.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(typeof(decimal), "0", "999999", ErrorMessage = "Weight must be >= 0.")]
        public decimal? Weight { get; set; }

        public bool Ismanual { get; set; } = true;

        [Required(ErrorMessage = "OrderIndex is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderIndex must be a positive number.")]
        public int? Orderindex { get; set; }
    }

    public class CriteriaUpdateRequest
    {
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(255, ErrorMessage = "Content cannot exceed 255 characters.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(typeof(decimal), "0", "999999", ErrorMessage = "Weight must be >= 0.")]
        public decimal? Weight { get; set; }

        public bool Ismanual { get; set; } = true;

        [Required(ErrorMessage = "OrderIndex is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderIndex must be a positive number.")]
        public int? Orderindex { get; set; }
    }
}