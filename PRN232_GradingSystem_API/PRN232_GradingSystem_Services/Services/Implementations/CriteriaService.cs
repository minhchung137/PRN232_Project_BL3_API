using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations;

public class CriteriaService : ICriteriaService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        
        public CriteriaService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<CriteriaBM> CreateAsync(CriteriaBM bm)
        {
            var questionRepo = _uow.Repository<Question>();
            var criteriaRepo = _uow.Repository<Criterion>();

            var question = await questionRepo.GetByIdAsync(bm.Questionid);
            if (question == null)
                throw new Exception("Question not found");

            // Check OrderIndex trùng
            var orderExist = await criteriaRepo.CountAsync(
                x => x.QuestionId == bm.Questionid && x.OrderIndex == bm.Orderindex
            );

            if (orderExist > 0)
                throw new Exception("OrderIndex already exists");

            // Check tổng weight
            var criteriaList = await criteriaRepo.GetAllAsync(
                x => x.QuestionId == bm.Questionid
            );

            var totalWeight = criteriaList.Sum(x => x.Weight);

            if (totalWeight + bm.Weight > question.MaxScore)
                throw new Exception("Total criteria weight exceeds question max score");

            var entity = new Criterion
            {
                QuestionId = bm.Questionid,
                Content = bm.Content,
                Weight = bm.Weight,
                IsManual = bm.Ismanual,
                OrderIndex = bm.Orderindex
            };

            await criteriaRepo.AddAsync(entity);
            await _uow.SaveChangesAsync();

            bm.Criteriaid = entity.CriteriaId;
            return bm;
        }

        public async Task<List<CriteriaBM>> GetByQuestionAsync(int questionId)
        {
            var list = await _uow.Repository<Criterion>()
                .GetAllAsync(
                    predicate: x => x.QuestionId == questionId,
                    orderBy: q => q.OrderBy(x => x.OrderIndex)
                );

            return list.Select(x => new CriteriaBM
            {
                Criteriaid = x.CriteriaId,
                Questionid = x.QuestionId ?? 0,
                Content = x.Content ?? "",
                Weight = x.Weight ,
                Ismanual = x.IsManual ?? true,
                Orderindex = x.OrderIndex
            }).ToList();
        }

        public async Task<bool> UpdateAsync(int criteriaId, CriteriaBM bm)
        {
            var repo = _uow.Repository<Criterion>();
            var entity = await repo.GetByIdAsync(criteriaId, trackChanges: true);

            if (entity == null) return false;

            entity.Content = bm.Content;
            entity.Weight = bm.Weight;
            entity.IsManual = bm.Ismanual;
            entity.OrderIndex = bm.Orderindex;

            repo.Update(entity);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int criteriaId)
        {
            var repo = _uow.Repository<Criterion>();
            var entity = await repo.GetByIdAsync(criteriaId, trackChanges: true);

            if (entity == null) return false;

            repo.Delete(entity);
            await _uow.SaveChangesAsync();
            return true;
        }
        
        public async Task<CriteriaBM?> GetByIdAsync(int criteriaId)
        {
            var entity = await _uow.Repository<Criterion>()
                .GetByIdAsync(criteriaId);

            return entity == null ? null : _mapper.Map<CriteriaBM>(entity);
        }
    }