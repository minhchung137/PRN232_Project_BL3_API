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
    [Route("api/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _service;
        private readonly IMapper _mapper;

        public SubjectController(ISubjectService service, IMapper mapper)
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
        // GET: api/subject/filter
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<SubjectResponse>>>> GetFilter([FromQuery] SubjectFilterRequest request)
        {
            try
            {
                var filter = _mapper.Map<SubjectBM>(request);
                var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
                var response = _mapper.Map<PagedResponse<SubjectResponse>>(paged);

                return Ok(ApiResponse<PagedResponse<SubjectResponse>>.SuccessResponse(response, "Subjects retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PagedResponse<SubjectResponse>>.FailResponse($"An error occurred: {ex.Message}", 500));
            }
        }

        // GET: api/subject/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id, includeDetails: true);
                if (item == null)
                    return NotFound(ApiResponse<SubjectResponse>.FailResponse($"Subject with ID {id} not found", 404));

                var response = _mapper.Map<SubjectResponse>(item);
                return Ok(ApiResponse<SubjectResponse>.SuccessResponse(response, "Subject retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<SubjectResponse>.FailResponse($"An error occurred: {ex.Message}", 500));
            }
        }


        // POST: api/subject
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SubjectResponse>>> Create(SubjectRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<SubjectBM>(request));
                var response = _mapper.Map<SubjectResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Subjectid },
                    ApiResponse<SubjectResponse>.SuccessResponse(response, "Subject created successfully", 201)
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

        // PUT: api/subject/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<SubjectResponse>>> Update(int id, SubjectRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, _mapper.Map<SubjectBM>(request));

                if (updated == null)
                    return NotFound(ApiResponse<SubjectResponse>.FailResponse(
                        $"Subject with ID {id} not found.", 404));

                var response = _mapper.Map<SubjectResponse>(updated);
                return Ok(ApiResponse<SubjectResponse>.SuccessResponse(response, "Subject updated successfully"));
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


        // DELETE: api/subject/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                //var ok = await _service.DeleteAsync(id);
                //if (!ok)
                //{
                //    return NotFound(ApiResponse<string>.FailResponse(
                //        $"Subject with ID {id} not found.", 404));
                //}

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse(
                    "An unexpected error occurred while deleting subject.", 500));
            }
        }

    }
}
