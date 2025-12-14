using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces;

public interface ICriteriaRepository : IGenericRepository<Criterion>
{
    Task<List<Criterion>> GetByQuestionIdAsync(int questionId);
}