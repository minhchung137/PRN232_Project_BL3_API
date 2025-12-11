using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class GradeWithDetailsRequestBM
    {

        public int? Submissionid { get; set; }

        public string? Marker { get; set; }
        public string? Comment { get; set; }
        public List<GradedetailBM> Gradedetails { get; set; } = new();
    }
}
