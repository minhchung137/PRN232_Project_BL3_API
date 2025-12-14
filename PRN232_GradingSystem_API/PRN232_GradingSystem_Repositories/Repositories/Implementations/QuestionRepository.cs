using Microsoft.EntityFrameworkCore;
using PRN232_GradingSystem_Repositories.DBContext;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.Repositories.Interfaces;
using ProductSaleApp.Repository.Repositories.Implementations;

namespace PRN232_GradingSystem_Repositories.Repositories.Implementations;

public class QuestionRepository 
    : GenericRepository<Question>, IQuestionRepository
{
    private readonly PRN232_GradingSystem_APIContext _context;

    public QuestionRepository(PRN232_GradingSystem_APIContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<Question>> GetByExamIdAsync(int examId)
    {
        return _context.Questions
            .Where(x => x.ExamId == examId)
            .OrderBy(x => x.QCode)
            .ToListAsync();
    }
}