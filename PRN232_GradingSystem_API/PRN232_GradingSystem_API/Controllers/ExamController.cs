using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/exams")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _service;
        private readonly IMapper _mapper;
        private readonly IExamExportService _exportService;

        public ExamController(IExamService service, IMapper mapper, IExamExportService exportService)
        {
            _service = service;
            _mapper = mapper;
            _exportService = exportService;
        }

        [HttpGet("export/{examId}")]
        public async Task<IActionResult> ExportExamScores(int examId)
        {
            try
            {
                // Lấy thông tin kỳ thi
                var exam = await _service.GetByIdAsync(examId, includeDetails: true);
                if (exam == null)
                    return NotFound(ApiResponse<string>.FailResponse($"Exam with ID {examId} not found"));

                var fileBytes = await _exportService.ExportExamScoresToExcelAsync(examId);

                // Lấy tên kỳ thi và ngày thi
                var fileName = $"{exam.Examname}_Marking_Sheet.xlsx";

                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(
                    $"Error exporting exam: {ex.Message}"));
            }
        }
        [EnableQuery]
        [HttpGet("odata")]
        public IActionResult GetOData()
        {
            var query = _service.GetODataQueryable();
            return Ok(query);
        }
        // ===== GET: api/exam/filter =====
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<ExamResponse>>>> GetFilter([FromQuery] ExamFilterRequest request)
        {
            var filter = _mapper.Map<ExamBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
            var response = _mapper.Map<PagedResponse<ExamResponse>>(paged);

            return Ok(ApiResponse<PagedResponse<ExamResponse>>.SuccessResponse(
                response, "Filtered exam list retrieved successfully"));
        }

        // ===== GET: api/exam/{id} =====
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ExamResponse>>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null)
                return NotFound(ApiResponse<ExamResponse>.FailResponse($"Exam with ID {id} not found.", 404));

            var response = _mapper.Map<ExamResponse>(item);
            return Ok(ApiResponse<ExamResponse>.SuccessResponse(response, "Exam retrieved successfully"));
        }

        // ===== POST: api/exam =====
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExamResponse>>> Create(ExamRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<ExamBM>(request));
                var response = _mapper.Map<ExamResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Examid },
                    ApiResponse<ExamResponse>.SuccessResponse(response, "Exam created successfully", 201)
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<ExamResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<ExamResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<ExamResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500, ApiResponse<ExamResponse>.FailResponse(
                    "An unexpected error occurred while creating the exam.", 500));
            }
        }


        // ===== PUT: api/exam/{id} =====
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<ExamResponse>>> Update(int id, ExamRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, _mapper.Map<ExamBM>(request));
                if (updated == null)
                    return NotFound(ApiResponse<ExamResponse>.FailResponse($"Exam with ID {id} not found.", 404));

                var response = _mapper.Map<ExamResponse>(updated);
                return Ok(ApiResponse<ExamResponse>.SuccessResponse(response, "Exam updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<ExamResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<ExamResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<ExamResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500, ApiResponse<ExamResponse>.FailResponse(
                    "An unexpected error occurred while updating the exam.", 500));
            }
        }


        // ===== DELETE: api/exam/{id} =====
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                //var ok = await _service.DeleteAsync(id);
                //if (!ok)
                //    return NotFound(ApiResponse<string>.FailResponse($"Exam with ID {id} not found.", 404));

               return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse(
                    "An unexpected error occurred while deleting the exam.", 500));
            }
        }
    }
}
