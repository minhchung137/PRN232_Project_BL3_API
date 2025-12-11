using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;
        private readonly IMapper _mapper;

        public StudentController(IStudentService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        [EnableQuery]
        [HttpGet("odata")]
        public IActionResult GetOData()
        {
            return Ok(_service.GetODataQueryable());
        }

        // ===== GET: api/student/filter =====
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<StudentResponse>>>> GetFilter([FromQuery] StudentFilterRequest request)
        {
            var filter = _mapper.Map<StudentBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
            var response = _mapper.Map<PagedResponse<StudentResponse>>(paged);

            return Ok(ApiResponse<PagedResponse<StudentResponse>>.SuccessResponse(
                response, "Filtered student list retrieved successfully"));
        }

        // ===== GET: api/student/{id} =====
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<StudentResponse>>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null)
                return NotFound(ApiResponse<StudentResponse>.FailResponse("Student not found", 404));

            var response = _mapper.Map<StudentResponse>(item);
            return Ok(ApiResponse<StudentResponse>.SuccessResponse(response, "Student retrieved successfully"));
        }

        // ===== POST: api/student =====
        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentResponse>>> Create(StudentRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<StudentBM>(request));
                var response = _mapper.Map<StudentResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Studentid },
                    ApiResponse<StudentResponse>.SuccessResponse(response, "Student created successfully", 201)
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GroupResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GroupResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<GroupResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500, ApiResponse<GroupResponse>.FailResponse(
                    "An unexpected error occurred while creating the group.", 500));
            }
        }

        // ===== PUT: api/student/{id} =====
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<StudentResponse>>> Update(int id, StudentRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, _mapper.Map<StudentBM>(request));
                if (updated == null)
                    return NotFound(ApiResponse<StudentResponse>.FailResponse($"Student with ID {id} not found.", 404));

                var response = _mapper.Map<StudentResponse>(updated);
                return Ok(ApiResponse<StudentResponse>.SuccessResponse(response, "Student updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GroupResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GroupResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<GroupResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500, ApiResponse<GroupResponse>.FailResponse(
                    "An unexpected error occurred while creating the group.", 500));
            }
        }

        // ===== DELETE: api/student/{id} =====
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var ok = await _service.DeactivateAsync(id);
                if (!ok)
                    return NotFound(ApiResponse<string>.FailResponse($"Student with ID {id} not found.", 404));

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse(
                    "An unexpected error occurred while deleting the student.", 500));
            }
        }
    }
}
