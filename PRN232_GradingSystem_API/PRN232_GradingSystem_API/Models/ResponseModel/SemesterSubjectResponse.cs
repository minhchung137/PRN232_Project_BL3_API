// Response model
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class SemesterSubjectResponse
    {
        public int Semesterid { get; set; }
        public int Subjectid { get; set; }
        public DateTime? Createat { get; set; }

        // Optional navigation
        public Semester Semester { get; set; }
        public Subject Subject { get; set; }
    }
}