using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class StudentAppealRequest
    {
        [Required]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [MinLength(10, ErrorMessage = "Reason must be at least 10 characters")]
        public string Reason { get; set; } = string.Empty;
    }
}