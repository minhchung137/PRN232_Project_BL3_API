namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class ModeratorReviewRequestBM
    {
        public int GradeId { get; set; }

        public decimal? Q1 { get; set; }
        public decimal? Q2 { get; set; }
        public decimal? Q3 { get; set; }
        public decimal? Q4 { get; set; }
        public decimal? Q5 { get; set; }
        public decimal? Q6 { get; set; }

        public string Note { get; set; }
    }
}