using System;
using System.Collections.Generic;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class SemesterBM
    {
        public int Semesterid { get; set; }
        public string Semestercode { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Createat { get; set; }
        public DateTime? Updateat { get; set; }
        public int? Createby { get; set; }
        public int? Updateby { get; set; }

        public virtual AppUser CreateByNavigation { get; set; }
        public virtual AppUser UpdateByNavigation { get; set; }
        public IReadOnlyList<Exam> Exams { get; set; }
        public IReadOnlyList<ClassGroup> Groups { get; set; }
        public IReadOnlyList<SemesterSubject> SemesterSubjects { get; set; }
    }
}
