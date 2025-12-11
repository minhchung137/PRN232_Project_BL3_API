using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class ModeratorReviewRequest
    {
        [Required]
        public int GradeId { get; set; }

        [Range(0, 10)]
        public decimal? Q1 { get; set; }
        [Range(0, 10)]
        public decimal? Q2 { get; set; }
        [Range(0, 10)]
        public decimal? Q3 { get; set; }
        [Range(0, 10)]
        public decimal? Q4 { get; set; }
        [Range(0, 10)]
        public decimal? Q5 { get; set; }
        [Range(0, 10)]
        public decimal? Q6 { get; set; }

        [Required(ErrorMessage = "Moderator note is required.")]
        public string Note { get; set; } = string.Empty;
    }
}