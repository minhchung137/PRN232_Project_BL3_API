using PRN232_GradingSystem_Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    public class GroupStudentFilterRequest
    {
        public int? Groupid { get; set; }
        public int? Studentid { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class GroupStudentRequest
    {
        [Required(ErrorMessage = "Group ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Group ID must be a positive integer.")]
        public int Groupid { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Student ID must be a positive integer.")]
        public int Studentid { get; set; }
    }
}
