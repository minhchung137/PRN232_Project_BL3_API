namespace PRN232_GradingSystem_Services.BusinessModel;

public class CriteriaRubricBM
{
    public int Criteriaid { get; set; }
    public int Orderindex { get; set; }
    public string Content { get; set; } = "";
    public decimal Weight { get; set; }
    public bool Ismanual { get; set; }
}