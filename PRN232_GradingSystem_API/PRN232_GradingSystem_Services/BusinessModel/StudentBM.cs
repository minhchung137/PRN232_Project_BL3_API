using System;
using System.Collections.Generic;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class StudentBM
    {
        public int Studentid { get; set; }
        public string Studentfullname { get; set; }
        public string Studentroll { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Createat { get; set; }

        public IReadOnlyList<GroupStudent> GroupStudents { get; set; }
        public IReadOnlyList<Submission> Submissions { get; set; }
    }
}
