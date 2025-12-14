using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using PRN232_GradingSystem_Repositories.DBContext;
using PRN232_GradingSystem_Repositories.Repositories.Implementations;
using PRN232_GradingSystem_Repositories.Repositories.Interfaces;
using ProductSaleApp.Repository.Repositories.Implementations;

namespace PRN232_GradingSystem_Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        // Strong-typed repositories backing fields
        private IExamRepository _examRepository;
        private IGradeRepository _gradeRepository;
        private IGradedetailRepository _gradedetailRepository;
        private IGroupRepository _groupRepository;
        private IGroupStudentRepository _groupStudentRepository;
        private ISemesterRepository _semesterRepository;
        private ISemesterSubjectRepository _semesterSubjectRepository;
        private IStudentRepository _studentRepository;
        private ISubjectRepository _subjectRepository;
        private ISubmissionRepository _submissionRepository;
        private IUserRepository _userRepository;

        private IQuestionRepository _questionRepository;
        private ICriteriaRepository _criteriaRepository;
        
        public UnitOfWork(PRN232_GradingSystem_APIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PRN232_GradingSystem_APIContext DbContext => _dbContext;

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (_repositories.ContainsKey(type))
            {
                return (IGenericRepository<TEntity>)_repositories[type];
            }

            var repository = new GenericRepository<TEntity>(_dbContext);
            _repositories[type] = repository;

            return repository;
        }

        public IExamRepository ExamRepository => _examRepository ??= new ExamRepository(_dbContext);
        public IGradeRepository GradeRepository => _gradeRepository ??= new GradeRepository(_dbContext);
        public IGradedetailRepository GradedetailRepository => _gradedetailRepository ??= new GradedetailRepository(_dbContext);
        public IGroupRepository GroupRepository => _groupRepository ??= new GroupRepository(_dbContext);
        public IGroupStudentRepository GroupStudentRepository => _groupStudentRepository ??= new GroupStudentRepository(_dbContext);
        public ISemesterRepository SemesterRepository => _semesterRepository ??= new SemesterRepository(_dbContext);
        public ISemesterSubjectRepository SemesterSubjectRepository => _semesterSubjectRepository ??= new SemesterSubjectRepository(_dbContext);
        public IStudentRepository StudentRepository => _studentRepository ??= new StudentRepository(_dbContext);
        public ISubjectRepository SubjectRepository => _subjectRepository ??= new SubjectRepository(_dbContext);
        public ISubmissionRepository SubmissionRepository => _submissionRepository ??= new SubmissionRepository(_dbContext);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_dbContext);

        public IQuestionRepository QuestionRepository
            => _questionRepository ??= new QuestionRepository(_dbContext);

        public ICriteriaRepository CriteriaRepository
            => _criteriaRepository ??= new CriteriaRepository(_dbContext);
        
        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
