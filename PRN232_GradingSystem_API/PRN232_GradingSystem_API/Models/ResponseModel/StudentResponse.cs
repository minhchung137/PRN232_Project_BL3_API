using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class StudentResponse
    {
        public int Studentid { get; set; }

        public string Studentfullname { get; set; }

        public string Studentroll { get; set; }

        public bool? Isactive { get; set; }

        public DateTime? Createat { get; set; }

        // Optional navigation
        //public IReadOnlyList<GroupStudent> GroupStudents { get; set; }
        //public IReadOnlyList<Submission> Submissions { get; set; }
    }
}
