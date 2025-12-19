using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/semester-subjects")]
    public class SemesterSubjectController : ControllerBase
    {
        private readonly ISemesterSubjectService _service;
        private readonly IMapper _mapper;

        public SemesterSubjectController(ISemesterSubjectService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        [EnableQuery]
        [HttpGet("odata")]
        public IActionResult GetOData()
        {
            var query = _service.GetODataQueryable();
            return Ok(query);
        }
        // GET: api/semestersubject/filter
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<SemesterSubjectResponse>>>> GetFilter([FromQuery] SemesterSubjectFilterRequest request)
        {
            try
            {
                var filter = _mapper.Map<SemesterSubjectBM>(request);
                var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
                var response = _mapper.Map<PagedResponse<SemesterSubjectResponse>>(paged);
                return Ok(ApiResponse<PagedResponse<SemesterSubjectResponse>>.SuccessResponse(response, "Filtered semester-subjects retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PagedResponse<SemesterSubjectResponse>>.FailResponse(
                    "An unexpected error occurred while retrieving semester-subjects.", 500));
            }
        }

        // POST: api/semestersubject
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SemesterSubjectResponse>>> Create(SemesterSubjectRequest request)
        {
            try
            {
                var model = _mapper.Map<SemesterSubjectBM>(request);
                var created = await _service.CreateAsync(model);
                var response = _mapper.Map<SemesterSubjectResponse>(created);

                return StatusCode(201, ApiResponse<SemesterSubjectResponse>.SuccessResponse(
                    response, "Semester-Subject created successfully", 201
                ));
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

        //// PUT: api/semestersubject/{id}
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult<ApiResponse<SemesterSubjectResponse>>> Update(int id, SemesterSubjectFilterRequest request)
        //{
        //    try
        //    {
        //        var updated = await _service.UpdateAsync(id, _mapper.Map<SemesterSubjectBM>(request));
        //        if (updated == null)
        //            return NotFound(ApiResponse<SemesterSubjectResponse>.FailResponse(
        //                $"Semester-Subject with ID {id} not found.", 404));

        //        var response = _mapper.Map<SemesterSubjectResponse>(updated);
        //        return Ok(ApiResponse<SemesterSubjectResponse>.SuccessResponse(response, "Semester-Subject updated successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        //            return BadRequest(ApiResponse<SemesterSubjectResponse>.FailResponse(ex.Message, 400));

        //        return StatusCode(500, ApiResponse<SemesterSubjectResponse>.FailResponse(
        //            "An unexpected error occurred while updating semester-subject.", 500));
        //    }
        //}

        // DELETE: api/semestersubject?semesterId=1&subjectId=2
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] int semesterId, [FromQuery] int subjectId)
        {
            try
            {
                var ok = await _service.DeleteAsync(semesterId, subjectId);
                if (!ok)
                    return NotFound(ApiResponse<string>.FailResponse(
                        $"Semester-Subject with SemesterId {semesterId} and SubjectId {subjectId} not found.", 404));

                return NoContent();
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


    }
}
