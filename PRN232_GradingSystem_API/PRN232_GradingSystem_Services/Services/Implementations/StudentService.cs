using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class StudentService : CrudService<Student, StudentBM>, IStudentService
    {
        private readonly IMapper _mapper;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<Student>
            GetRepository() => UnitOfWork.StudentRepository;

        public async Task<PagedResult<StudentBM>> GetPagedFilteredAsync(StudentBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.StudentRepository;

            var repositoryFilter = new Student
            {
                Studentid = filter?.Studentid ?? 0,
                Studentfullname = filter?.Studentfullname,
                Studentroll = filter?.Studentroll,
                Isactive = filter?.Isactive
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<StudentBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<StudentBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public IQueryable<StudentBM> GetODataQueryable()
        {
            var repo = UnitOfWork.StudentRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<StudentBM>(query);
        }

        public override async Task<StudentBM> CreateAsync(StudentBM model)
        {
            var repo = UnitOfWork.StudentRepository;
            var groupStudentRepo = UnitOfWork.GroupStudentRepository;


            // Kiểm tra trùng StudentRoll trong toàn hệ thống
            var exists = await repo.ExistsByRollAsync(model.Studentroll, excludeId: 0);
            if (exists)
                throw new Exception($"Student roll '{model.Studentroll}' already exists.");

            // ===== MAP & SAVE =====
            var entity = _mapper.Map<Student>(model);
            entity.Createat = DateTime.UtcNow;
            entity.Isactive ??= true;

            await repo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            // ===== MAP NGƯỢC LẠI =====
            return _mapper.Map<StudentBM>(entity);
        }

        public override async Task<StudentBM> UpdateAsync(int id, StudentBM model)
        {
            var repo = UnitOfWork.StudentRepository;
            var groupStudentRepo = UnitOfWork.GroupStudentRepository;

            // ===== LẤY DỮ LIỆU GỐC =====
            var existing = await repo.GetByIdAsync(id);
            if (existing == null)
                throw new Exception($"Student with ID '{id}' does not exist.");

            var exists = await repo.ExistsByRollAsync(model.Studentroll, excludeId: id);
            if (exists)
                throw new Exception($"Student roll '{model.Studentroll}' already exists.");


            // ===== CẬP NHẬT =====
            existing.Studentfullname = model.Studentfullname ?? existing.Studentfullname;
            existing.Studentroll = model.Studentroll ?? existing.Studentroll;
            existing.Isactive = model.Isactive ?? existing.Isactive;
            existing.Createat ??= existing.Createat; // giữ nguyên

            repo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<StudentBM>(existing);
        }
        public async Task<bool> DeactivateAsync(int id)
        {
            var repo = UnitOfWork.StudentRepository;

            // ===== LẤY DỮ LIỆU GỐC =====
            var existing = await repo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
                return false;

            // ===== CẬP NHẬT TRẠNG THÁI =====
            existing.Isactive = false;
            existing.Studentfullname = existing.Studentfullname;
            existing.Studentroll = existing.Studentroll;
            existing.Createat ??= existing.Createat; // giữ nguyên

            repo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return true;
        }

    }
}
