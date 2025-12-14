namespace PRN232_GradingSystem_Services.BusinessModel
{
    public class CriteriaBM
    {
        public int Criteriaid { get; set; }
        public int Questionid { get; set; }
        public string Content { get; set; }
        public decimal Weight { get; set; }
        public bool Ismanual { get; set; }
        public int Orderindex { get; set; }
    }
}