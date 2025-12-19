using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class StudentFilterRequest
    {
        public int? Studentid { get; set; } = 1;
        [DefaultValue("John Doe")]
        public string? Studentfullname { get; set; } 
        [DefaultValue("SE123456")]
        public string? Studentroll { get; set; }
        public bool? Isactive { get; set; } = true;

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class StudentRequest
    {
        [Required(ErrorMessage = "Student full name is required.")]
        [DefaultValue("John Doe")]
        [StringLength(100, ErrorMessage = "Student full name must be less than 100 characters.")]
        public string Studentfullname { get; set; }

        [Required(ErrorMessage = "Student roll is required.")]
        [RegularExpression(@"^[A-Z]{2}\d{6}$", ErrorMessage = "Student roll must start with 2 uppercase letters followed by 6 digits (e.g., SE183335).")]
        [DefaultValue("SE123456")]
        public string Studentroll { get; set; }

        public bool? Isactive { get; set; } = true;
    }

}