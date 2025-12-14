namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class CriteriaResponse
    {
        public int Criteriaid { get; set; }
        public int Questionid { get; set; }
        public string Content { get; set; }
        public decimal Weight { get; set; }
        public bool Ismanual { get; set; }
        public int Orderindex { get; set; }
    }
}