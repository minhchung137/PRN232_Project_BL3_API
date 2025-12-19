using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class SemesterFilterRequest
    {
        [DefaultValue(1)]
        public int? Semesterid { get; set; }
        [DefaultValue("FALL25")]
        public string? Semestercode { get; set; }

        [DefaultValue(true)]
        public bool? Isactive { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SemesterCreateRequest
    {
        [Required(ErrorMessage = "Semester code is required.")]
        [DefaultValue("FALL25")]
        [StringLength(50, ErrorMessage = "Semester code cannot exceed 50 characters.")]
        public string Semestercode { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DefaultValue("2025-09-01")]
        public DateTime? Startdate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DefaultValue("2025-12-31")]
        [DateGreaterThan(nameof(Startdate), ErrorMessage = "End date must be after start date.")]
        public DateTime? Enddate { get; set; }

        public bool? Isactive { get; set; } = true;

        [Required(ErrorMessage = "Creator by is required.")]
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "Creator by must be a valid positive integer.")]
        public int? Createby { get; set; }
    }
    public class SemesterUpdateRequest
    {
        [Required(ErrorMessage = "Semester code is required.")]
        [DefaultValue("FALL25")]
        [StringLength(50, ErrorMessage = "Semester code cannot exceed 50 characters.")]
        public string Semestercode { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DefaultValue("2025-09-01")]
        public DateTime? Startdate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DateGreaterThan(nameof(Startdate), ErrorMessage = "End date must be after start date.")]
        [DefaultValue("2025-12-31")]
        public DateTime? Enddate { get; set; }

        public bool? Isactive { get; set; } = true;

        [Required(ErrorMessage = "Updater by is required.")]
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "Updater by must be a valid positive integer.")]
        public int Updateby { get; set; }
    }
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = value as DateTime?;
            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (comparisonProperty == null)
                return new ValidationResult($"Unknown property: {_comparisonProperty}");

            var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (currentValue.HasValue && comparisonValue.HasValue && currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be after {_comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }
}
