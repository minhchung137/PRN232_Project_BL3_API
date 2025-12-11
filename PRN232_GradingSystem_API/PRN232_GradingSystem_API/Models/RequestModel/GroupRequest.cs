using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class GroupFilterRequest
    {
        public string? Groupname { get; set; }
        public int? Semesterid { get; set; }
        public int? Createby { get; set; }
        public int? Updateby { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class GroupCreateRequest
    {
        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name must be between 2 and 100 characters.")]
        public string Groupname { get; set; }

        [Required(ErrorMessage = "Semester ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Semester ID must be greater than 0.")]
        public int Semesterid { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Createby must be a valid user ID.")]
        public int? Createby { get; set; }
    }
    public class GroupUpdateRequest
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name must be between 2 and 100 characters.")]
        public string? Groupname { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Semester ID must be greater than 0.")]
        public int? Semesterid { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Updateby must be a valid user ID.")]
        public int? Updateby { get; set; }
    }
}