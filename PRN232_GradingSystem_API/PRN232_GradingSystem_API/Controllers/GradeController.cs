using AutoMapper;
using Azure;
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
    public class GradeController : ControllerBase
    {
        private readonly IGradeService _service;
        private readonly IMapper _mapper;

        public GradeController(IGradeService service, IMapper mapper)
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

        // GET: api/grade/filter
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<GradeResponse>>>> GetFilter([FromQuery] GradeFilterRequest request)
        {
            var filter = _mapper.Map<GradeBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
            var response = _mapper.Map<PagedResponse<GradeResponse>>(paged);

            return Ok(ApiResponse<PagedResponse<GradeResponse>>.SuccessResponse(
                response, "Filtered grade list retrieved successfully"));
        }

        // ===== GET: api/grade/{id} =====
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<GradeResponse>>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null)
                return NotFound(ApiResponse<GradeResponse>.FailResponse($"Grade with ID {id} not found.", 404));

            var response = _mapper.Map<GradeResponse>(item);
            return Ok(ApiResponse<GradeResponse>.SuccessResponse(response, "Grade retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<GradeResponse>>> Create(GradeRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<GradeBM>(request));
                var response = _mapper.Map<GradeResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Gradeid },
                    ApiResponse<GradeResponse>.SuccessResponse(response, "Grade created successfully", 201)
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GradeResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GradeResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<GradeResponse>.FailResponse(ex.Message, 409));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<GradeResponse>.FailResponse(
                    "An unexpected error occurred while creating the grade.", 500));
            }
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<GradeResponse>>> Update(int id, GradeRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, _mapper.Map<GradeBM>(request));
                if (updated == null)
                    return NotFound(ApiResponse<GradeResponse>.FailResponse($"Grade with ID {id} not found.", 404));

                var response = _mapper.Map<GradeResponse>(updated);
                return Ok(ApiResponse<GradeResponse>.SuccessResponse(response, "Grade updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GradeResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GradeResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<GradeResponse>.FailResponse(ex.Message, 409));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<GradeResponse>.FailResponse(
                    "An unexpected error occurred while updating the grade.", 500));
            }
        }


        // ===== DELETE: api/grade/{id} =====
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            //var ok = await _service.DeleteAsync(id);
            //if (!ok)
            //    return NotFound(ApiResponse<string>.FailResponse($"Grade with ID {id} not found.", 404));

            return NoContent();
        }

        [HttpPost("with-details")]
        public async Task<ActionResult<ApiResponse<GradeResponse>>> CreateWithDetails(GradeWithDetailsRequest request)
        {
            try
            {
                // ===== Map request → BM sử dụng AutoMapper =====
                var requestDto = _mapper.Map<GradeWithDetailsRequestBM>(request);

                // Truyền xuống service
                var created = await _service.CreateGradeWithDetailsAsync(requestDto);

                var response = _mapper.Map<GradeResponse>(created);
                return CreatedAtAction(nameof(GetById), new { id = response.Gradeid },
                    ApiResponse<GradeResponse>.SuccessResponse(response, "Grade with details created successfully", 201));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GradeResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GradeResponse>.FailResponse(ex.Message, 404));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<GradeResponse>.FailResponse(
                    "An unexpected error occurred while creating the grade with details.", 500));
            }
        }
        [HttpPut("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<GradeResponse>>> UpdateGradeStatus(
    int id,
    [FromQuery] string? status)
        {
            try
            {
                var updated = await _service.UpdateStatusAsync(id, status);
                if (updated == null)
                    return NotFound(ApiResponse<GradeResponse>.FailResponse(
                        $"Grade with ID {id} not found.", 404));

                var response = _mapper.Map<GradeResponse>(updated);
                return Ok(ApiResponse<GradeResponse>.SuccessResponse(response, "Grade status updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<GradeResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<GradeResponse>.FailResponse(ex.Message, 404));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<GradeResponse>.FailResponse(
                    "An unexpected error occurred while updating the grade.", 500));
            }
        }

    }
}
