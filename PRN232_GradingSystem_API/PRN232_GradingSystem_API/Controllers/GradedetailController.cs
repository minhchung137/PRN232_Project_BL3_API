using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class GradedetailController : ControllerBase
    {
        private readonly IGradedetailService _service;
        private readonly IMapper _mapper;

        public GradedetailController(IGradedetailService service, IMapper mapper)
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

        // GET: api/gradedetail/filter
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<GradedetailResponse>>>> GetFilter([FromQuery] GradedetailFilterRequest request)
        {
            var filter = _mapper.Map<GradedetailBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
            var response = _mapper.Map<PagedResponse<GradedetailResponse>>(paged);

            return Ok(ApiResponse<PagedResponse<GradedetailResponse>>.SuccessResponse(
                response, "Filtered gradedetail list retrieved successfully"));
        }

        // GET: api/gradedetail/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<GradedetailResponse>>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null)
                return NotFound(ApiResponse<GradedetailResponse>.FailResponse($"Gradedetail with ID {id} not found.", 404));

            var response = _mapper.Map<GradedetailResponse>(item);
            return Ok(ApiResponse<GradedetailResponse>.SuccessResponse(response, "Gradedetail retrieved successfully"));
        }

        // POST: api/gradedetail
        [HttpPost]
        public async Task<ActionResult<ApiResponse<GradedetailResponse>>> Create(GradedetailCreateRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<GradedetailBM>(request));
                var response = _mapper.Map<GradedetailResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Gradedetailid },
                    ApiResponse<GradedetailResponse>.SuccessResponse(response, "Gradedetail created successfully", 201)
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GradedetailResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GradedetailResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<GradedetailResponse>.FailResponse(ex.Message, 409));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<GradedetailResponse>.FailResponse(
                    "An unexpected error occurred while creating the gradedetail.", 500));
            }
        }

        // PUT: api/gradedetail
        [HttpPut]
        public async Task<ActionResult<ApiResponse<GradedetailResponse>>> Update(GradedetailUpdateRequestList request)
        {
            try
            {
                await _service.UpdateManyAsync(_mapper.Map<GradedetailUpdateRequestListBM>(request));
                return Ok(ApiResponse<string>.SuccessResponse(null, "Gradedetails updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GradedetailResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GradedetailResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<GradedetailResponse>.FailResponse(ex.Message, 409));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<GradedetailResponse>.FailResponse(
                    "An unexpected error occurred while updating the gradedetail.", 500));
            }
        }

        // DELETE: api/gradedetail/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                //var ok = await _service.DeleteAsync(id);
                //if (!ok)
                //    return NotFound(ApiResponse<string>.FailResponse($"Gradedetail with ID {id} not found.", 404));

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse(
                    "An unexpected error occurred while deleting the gradedetail.", 500));
            }
        }
    }
}
