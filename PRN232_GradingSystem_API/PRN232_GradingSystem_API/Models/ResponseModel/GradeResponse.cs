using System;
using System.Collections.Generic;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class GradeResponse
    {
        public int Gradeid { get; set; }

        public int? Submissionid { get; set; }

        public decimal? Q1 { get; set; }

        public decimal? Q2 { get; set; }

        public decimal? Q3 { get; set; }

        public decimal? Q4 { get; set; }

        public decimal? Q5 { get; set; }

        public decimal? Q6 { get; set; }

        public decimal? Totalscore { get; set; }
        public string? Status { get; set; }

        public DateTime? Createat { get; set; }

        public DateTime? Updateat { get; set; }

        public int? Marker { get; set; }
        // Navigation properties
        //public Submission Submission { get; set; }
        //public User MarkerNavigation { get; set; }
        //public IReadOnlyList<Gradedetail> Gradedetails { get; set; }
    }


}
