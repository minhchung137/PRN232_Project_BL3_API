using System;
using System.Collections.Generic;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class SubjectBM
    {
        public int Subjectid { get; set; }
        public string Subjectname { get; set; }
        public DateTime? Createat { get; set; }

        public IReadOnlyList<Exam> Exams { get; set; }
        public IReadOnlyList<SemesterSubject> SemesterSubjects { get; set; }
    }
}
