using System;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class SemesterSubjectBM
    {
        public int Semesterid { get; set; }
        public int Subjectid { get; set; }
        public DateTime? Createat { get; set; }

        public Semester Semester { get; set; }
        public Subject Subject { get; set; }
    }
}
