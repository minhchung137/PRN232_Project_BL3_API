using PRN232_GradingSystem_Repositories.DBContext;
using PRN232_GradingSystem_Repositories.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        PRN232_GradingSystem_APIContext DbContext { get; }

        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        // Strong-typed repositories
        IExamRepository ExamRepository { get; }
        IGradeRepository GradeRepository { get; }
        IGradedetailRepository GradedetailRepository { get; }
        IGroupRepository GroupRepository { get; }
        IGroupStudentRepository GroupStudentRepository { get; }
        ISemesterRepository SemesterRepository { get; }
        ISemesterSubjectRepository SemesterSubjectRepository { get; }
        IStudentRepository StudentRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        ISubmissionRepository SubmissionRepository { get; }
        IUserRepository UserRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
