namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class QuestionBM
    {
        public int Questionid { get; set; }
        public int Examid { get; set; }
        public string Qcode { get; set; }
        public string? Description { get; set; }
        public decimal Maxscore { get; set; }
    }
}