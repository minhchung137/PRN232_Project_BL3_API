using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
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
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> Create([FromBody] QuestionRequest req)
        {
            try
            {
                var bm = _mapper.Map<QuestionBM>(req);
                var created = await _service.CreateAsync(bm);
                var response = _mapper.Map<QuestionResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { questionId = response.Questionid },
                    ApiResponse<QuestionResponse>.SuccessResponse(
                        response, "Question created successfully", 201)
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<QuestionResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<QuestionResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<QuestionResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<QuestionResponse>.FailResponse(
                        "An unexpected error occurred while creating question.", 500));
            }
        }
        
        [Authorize]
        [HttpGet("by-exam/{examId:int}")]
        public async Task<ActionResult<ApiResponse<List<QuestionResponse>>>> GetByExam(int examId)
        {
            try
            {
                var list = await _service.GetByExamAsync(examId);
                var response = _mapper.Map<List<QuestionResponse>>(list);

                return Ok(ApiResponse<List<QuestionResponse>>.SuccessResponse(
                    response, "Questions retrieved successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<List<QuestionResponse>>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<List<QuestionResponse>>.FailResponse(ex.Message, 404));
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<List<QuestionResponse>>.FailResponse(
                        "An unexpected error occurred while retrieving questions.", 500));
            }
        }
        
        [HttpGet("{questionId:int}")]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> GetById(int questionId)
        {
            var item = await _service.GetByIdAsync(questionId);

            if (item == null)
            {
                return NotFound(ApiResponse<QuestionResponse>.FailResponse(
                    "Question not found", 404));
            }

            var response = _mapper.Map<QuestionResponse>(item);
            return Ok(ApiResponse<QuestionResponse>.SuccessResponse(
                response, "Question retrieved successfully"));
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{questionId:int}")]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> Update(
            int questionId,
            [FromBody] QuestionUpdateRequest req)
        {
            try
            {
                var bm = _mapper.Map<QuestionBM>(req);
                var ok = await _service.UpdateAsync(questionId, bm);

                if (!ok)
                {
                    return NotFound(ApiResponse<QuestionResponse>.FailResponse(
                        $"Question with ID {questionId} not found.", 404));
                }

                var updated = await _service.GetByIdAsync(questionId);
                var response = _mapper.Map<QuestionResponse>(updated);

                return Ok(ApiResponse<QuestionResponse>.SuccessResponse(
                    response, "Question updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<QuestionResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<QuestionResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<QuestionResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<QuestionResponse>.FailResponse(
                        "An unexpected error occurred while updating question.", 500));
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{questionId:int}")]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> Delete(int questionId)
        {
            try
            {
                var ok = await _service.DeleteAsync(questionId);

                if (!ok)
                {
                    return NotFound(ApiResponse<QuestionResponse>.FailResponse(
                        $"Question with ID {questionId} not found.", 404));
                }
                
                return NoContent();
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<QuestionResponse>.FailResponse(
                        "An unexpected error occurred while deleting question.", 500));
            }
        }
    }
}