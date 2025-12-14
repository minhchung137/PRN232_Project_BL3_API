using Microsoft.EntityFrameworkCore;
using PRN232_GradingSystem_Repositories.DBContext;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.Repositories.Interfaces;
using ProductSaleApp.Repository.Repositories.Implementations;

namespace PRN232_GradingSystem_Repositories.Repositories.Implementations;

public class CriteriaRepository 
    : GenericRepository<Criterion>, ICriteriaRepository
{
    private readonly PRN232_GradingSystem_APIContext _context;

    public CriteriaRepository(PRN232_GradingSystem_APIContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<Criterion>> GetByQuestionIdAsync(int questionId)
    {
        return _context.Criteria
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync();
    }
}