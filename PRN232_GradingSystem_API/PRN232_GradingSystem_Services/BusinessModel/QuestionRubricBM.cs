namespace PRN232_GradingSystem_Services.BusinessModel;

public class QuestionRubricBM
{
    public int Questionid { get; set; }
    public string Qcode { get; set; } = "";
    public string? Description { get; set; }
    public decimal Maxscore { get; set; }

    public List<CriteriaRubricBM> Criteria { get; set; } = new();
}