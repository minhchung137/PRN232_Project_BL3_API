// Response model
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class GroupStudentResponse
    {
        public int Groupid { get; set; }
        public int Studentid { get; set; }

        // Optional navigation properties
        //public Group Group { get; set; }
        //public Student Student { get; set; }
    }

}
