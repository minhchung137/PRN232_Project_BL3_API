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
    [Route("api/[controller]")]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _service;
        private readonly IMapper _mapper;

        public SemesterController(ISemesterService service, IMapper mapper)
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
        // GET: api/semester/filter
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<SemesterResponse>>>> GetFilter([FromQuery] SemesterFilterRequest request)
        {
            var filter = _mapper.Map<SemesterBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);

            var response = _mapper.Map<PagedResponse<SemesterResponse>>(paged);
            return Ok(ApiResponse<PagedResponse<SemesterResponse>>.SuccessResponse(response, "Filtered list retrieved successfully"));
        }

        // GET: api/semester/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null)
            {
                return NotFound(ApiResponse<SemesterResponse>.FailResponse("Semester not found", 404));
            }

            var response = _mapper.Map<SemesterResponse>(item);
            return Ok(ApiResponse<SemesterResponse>.SuccessResponse(response, "Semester retrieved successfully"));
        }


        // POST: api/semester
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SemesterResponse>>> Create(SemesterCreateRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<SemesterBM>(request));
                var response = _mapper.Map<SemesterResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Semesterid },
                    ApiResponse<SemesterResponse>.SuccessResponse(response, "Semester created successfully", 201)
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


        // PUT: api/semester/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<SemesterResponse>>> Update(int id, SemesterUpdateRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, _mapper.Map<SemesterBM>(request));

                if (updated == null)
                    return NotFound(ApiResponse<SemesterResponse>.FailResponse(
                        $"Semester with ID {id} not found.", 404));

                var response = _mapper.Map<SemesterResponse>(updated);
                return Ok(ApiResponse<SemesterResponse>.SuccessResponse(response, "Semester updated successfully"));
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

        //// DELETE: api/semester/{id}
        //[HttpDelete("{id:int}")]
        //public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        //{
        //    try
        //    {
        //        var result = await _service.DeleteAsync(id);

        //        if (!result)
        //        {
        //            return NotFound(ApiResponse<string>.FailResponse(
        //                $"Semester with ID {id} not found.", 404));
        //        }

        //        return Ok(ApiResponse<string>.SuccessResponse(
        //            null, $"Semester with ID {id} deleted successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        //        {
        //            return BadRequest(ApiResponse<string>.FailResponse(ex.Message, 400));
        //        }

        //        return StatusCode(500, ApiResponse<string>.FailResponse(
        //            "An unexpected error occurred while deleting semester.", 500));
        //    }
        //}

        // DELETE: api/semester/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<SemesterResponse>>> Delete(int id)
        {
            try
            {
                var updated = await _service.DeactivateAsync(id);

                if (updated == null)
                    return NotFound(ApiResponse<SemesterResponse>.FailResponse(
                        $"Semester with ID {id} not found.", 404));

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<SemesterResponse>.FailResponse(
                    "An unexpected error occurred while deactivating semester.", 500));
            }
        }

    }
}
