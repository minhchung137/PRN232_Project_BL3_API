namespace PRN232_GradingSystem_Services.BusinessModel;

public class ExamRubricBM
{
    public string Examcode { get; set; } = "";
    public List<QuestionRubricBM> Questions { get; set; } = new();
}