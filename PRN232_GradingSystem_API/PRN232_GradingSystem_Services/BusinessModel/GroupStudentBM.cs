using System;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class GroupStudentBM
    {
        public int Groupid { get; set; }
        public int Studentid { get; set; }

        public DateTime? Createat { get; set; }

        public ClassGroup Group { get; set; }
        public Student Student { get; set; }
    }
}
