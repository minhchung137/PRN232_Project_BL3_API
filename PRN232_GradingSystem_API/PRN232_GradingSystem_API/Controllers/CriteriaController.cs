using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_API.Controllers
{
    [ApiController]
    [Route("api/criteria")]
    public class CriteriaController : ControllerBase
    {
        private readonly ICriteriaService _service;
        private readonly IMapper _mapper;

        public CriteriaController(ICriteriaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // POST: api/criteria
        [HttpPost]
        public async Task<ActionResult<CriteriaResponse>> Create([FromBody] CriteriaRequest req)
        {
            var bm = _mapper.Map<CriteriaBM>(req);
            var created = await _service.CreateAsync(bm);
            return Ok(_mapper.Map<CriteriaResponse>(created));
        }

        // GET: api/criteria/by-question/5
        [HttpGet("by-question/{questionId}")]
        public async Task<ActionResult<List<CriteriaResponse>>> GetByQuestion(int questionId)
        {
            var list = await _service.GetByQuestionAsync(questionId);
            return Ok(_mapper.Map<List<CriteriaResponse>>(list));
        }

        // PUT: api/criteria/10
        [HttpPut("{criteriaId}")]
        public async Task<IActionResult> Update(int criteriaId, [FromBody] CriteriaUpdateRequest req)
        {
            var bm = _mapper.Map<CriteriaBM>(req);
            var ok = await _service.UpdateAsync(criteriaId, bm);
            return ok ? Ok() : NotFound();
        }

        // DELETE: api/criteria/10
        [HttpDelete("{criteriaId}")]
        public async Task<IActionResult> Delete(int criteriaId)
        {
            var ok = await _service.DeleteAsync(criteriaId);
            return ok ? Ok() : NotFound();
        }
    }
}