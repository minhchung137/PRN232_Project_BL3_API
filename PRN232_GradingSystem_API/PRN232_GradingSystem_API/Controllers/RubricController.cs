using Microsoft.AspNetCore.Mvc;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_API.Controllers
{
    [ApiController]
    [Route("api/rubrics")]
    public class RubricController : ControllerBase
    {
        private readonly IRubricService _service;

        public RubricController(IRubricService service)
        {
            _service = service;
        }

        [HttpGet("{examCode}")]
        public async Task<ActionResult<ApiResponse<object>>> GetByExamCode(string examCode)
        {
            try
            {
                var rubric = await _service.GetRubricByExamCodeAsync(examCode);
                return Ok(ApiResponse<object>.SuccessResponse(rubric, "Rubric retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.FailResponse(ex.Message, 404));
            }
        }
    }
}