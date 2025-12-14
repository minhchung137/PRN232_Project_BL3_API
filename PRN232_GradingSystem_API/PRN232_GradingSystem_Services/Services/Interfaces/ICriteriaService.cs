using PRN232_GradingSystem_Services.BusinessModel;

namespace PRN232_GradingSystem_Services.Services.Interfaces;

public interface ICriteriaService
{
    Task<CriteriaBM> CreateAsync(CriteriaBM bm);
    Task<List<CriteriaBM>> GetByQuestionAsync(int questionId);
    Task<bool> UpdateAsync(int criteriaId, CriteriaBM bm);
    Task<bool> DeleteAsync(int criteriaId);
}