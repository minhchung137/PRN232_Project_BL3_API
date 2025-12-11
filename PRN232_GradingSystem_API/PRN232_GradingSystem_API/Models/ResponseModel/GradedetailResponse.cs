using System;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class GradedetailResponse
    {
        public int Gradedetailid { get; set; }
        public int? Gradeid { get; set; }

        public string Qcode { get; set; }
        public string Subcode { get; set; }
        public decimal? Point { get; set; }
        public string Note { get; set; }

        public DateTime? Createat { get; set; }
        public DateTime? Updateat { get; set; }

        //public Grade Grade { get; set; }
    }
}
