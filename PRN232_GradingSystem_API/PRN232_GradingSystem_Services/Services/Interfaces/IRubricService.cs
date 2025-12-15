using PRN232_GradingSystem_Services.BusinessModel;

namespace PRN232_GradingSystem_Services.Services.Interfaces;

public interface IRubricService
{
    Task<ExamRubricBM> GetRubricByExamCodeAsync(string examCode);
}