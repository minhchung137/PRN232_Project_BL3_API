using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class GradeManualPartRequest
    {
        [Required]
        public int GradeId { get; set; }

        [Range(0, 1, ErrorMessage = "Q7 must be between 0 and 1.")]
        public decimal? Q7 { get; set; }
        [Range(0, 0.25, ErrorMessage = "Q8 must be between 0 and 0.25.")]
        public decimal? Q8 { get; set; }
        [Range(0, 0.25, ErrorMessage = "Q9 must be between 0 and 0.25.")]
        public decimal? Q9 { get; set; }
        [Range(0, 1.25, ErrorMessage = "Q10 must be between 0 and 1.25.")]
        public decimal? Q10 { get; set; }
        [Range(0, 0.75, ErrorMessage = "Q11 must be between 0 and 0.75.")]
        public decimal? Q11 { get; set; }
        [Range(0, 0.5, ErrorMessage = "Q12 must be between 0 and 0.5.")]
        public decimal? Q12 { get; set; }

        [Required(ErrorMessage = "Moderator note is required.")]
        public string Note { get; set; } = string.Empty;
    }
}
