using System;
using System.ComponentModel.DataAnnotations;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class GradedetailBM
    {
        public int Gradedetailid { get; set; }
        public int? Gradeid { get; set; }

        public string Qcode { get; set; }
        public string Subcode { get; set; }
        public decimal? Point { get; set; }
        public string Note { get; set; }

        public DateTime? Createat { get; set; }
        public DateTime? Updateat { get; set; }

        public Grade Grade { get; set; }
    }
    public class GradedetailUpdateItemBM
    {
        public int Gradedetailid { get; set; }

        public decimal? Point { get; set; }

        public string Note { get; set; }

    }
    public class GradedetailUpdateRequestListBM
    {
        public List<GradedetailUpdateItemBM> Gradedetails { get; set; } = new();
    }
}
