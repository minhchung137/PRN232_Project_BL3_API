using System;
using System.Collections.Generic;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class SubmissionBM
    {
        public int Submissionid { get; set; }
        public int? Examid { get; set; }
        public int? Studentid { get; set; }
        public string Solution { get; set; }
        public string Comment { get; set; }
        public string Fileurl { get; set; }
        public DateTime? Createat { get; set; }
        public DateTime? Updateat { get; set; }

        public Exam Exam { get; set; }
        public Student Student { get; set; }
        public IReadOnlyList<Grade> Grades { get; set; }
    }
}
