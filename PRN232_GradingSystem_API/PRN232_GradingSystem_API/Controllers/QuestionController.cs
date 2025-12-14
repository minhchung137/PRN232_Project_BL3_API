using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_API.Controllers
{
    [ApiController]
    [Route("api/questions")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _service;
        private readonly IMapper _mapper;

        public QuestionController(IQuestionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // POST: api/questions
        [HttpPost]
        public async Task<ActionResult<QuestionResponse>> Create([FromBody] QuestionRequest req)
        {
            var bm = _mapper.Map<QuestionBM>(req);
            var created = await _service.CreateAsync(bm);
            return Ok(_mapper.Map<QuestionResponse>(created));
        }

        // GET: api/questions/by-exam/5
        [HttpGet("by-exam/{examId}")]
        public async Task<ActionResult<List<QuestionResponse>>> GetByExam(int examId)
        {
            var list = await _service.GetByExamAsync(examId);
            return Ok(_mapper.Map<List<QuestionResponse>>(list));
        }

        // PUT: api/questions/10
        [HttpPut("{questionId}")]
        public async Task<IActionResult> Update(int questionId, [FromBody] QuestionUpdateRequest req)
        {
            var bm = _mapper.Map<QuestionBM>(req);
            var ok = await _service.UpdateAsync(questionId, bm);
            return ok ? Ok() : NotFound();
        }

        // DELETE: api/questions/10
        [HttpDelete("{questionId}")]
        public async Task<IActionResult> Delete(int questionId)
        {
            var ok = await _service.DeleteAsync(questionId);
            return ok ? Ok() : NotFound();
        }
    }
}