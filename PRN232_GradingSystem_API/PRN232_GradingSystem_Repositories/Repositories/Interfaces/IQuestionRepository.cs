using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces;

public interface IQuestionRepository : IGenericRepository<Question>
{
    Task<List<Question>> GetByExamIdAsync(int examId);
    
}