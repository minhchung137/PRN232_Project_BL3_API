using System;
using System.Collections.Generic;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class ExamResponse
    {
        public int Examid { get; set; }

        public int? Semesterid { get; set; }

        public int? Subjectid { get; set; }

        public string Examname { get; set; }

        public DateTime? Examdate { get; set; }

        public DateTime? Createat { get; set; }

        // Optional navigation properties
        //public Semester Semester { get; set; }
        //public Subject Subject { get; set; }

        //public IReadOnlyList<Submission> Submissions { get; set; }
    }


}
