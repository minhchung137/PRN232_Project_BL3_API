using PRN232_GradingSystem_Services.BusinessModel;

namespace PRN232_GradingSystem_Services.Services.Interfaces;

public interface IQuestionService
{
    Task<QuestionBM> CreateAsync(QuestionBM bm);
    Task<List<QuestionBM>> GetByExamAsync(int examId);
    Task<bool> UpdateAsync(int questionId, QuestionBM bm);
    Task<bool> DeleteAsync(int questionId);
}