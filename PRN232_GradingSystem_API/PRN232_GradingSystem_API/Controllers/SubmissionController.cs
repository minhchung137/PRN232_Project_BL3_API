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
    [Route("api/submissions")]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _service;
        private readonly IMapper _mapper;

        public SubmissionController(ISubmissionService service, IMapper mapper)
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

        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<SubmissionResponse>>>> GetFilter([FromQuery] SubmissionFilterRequest request)
        {
            var filter = _mapper.Map<SubmissionBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
            var response = _mapper.Map<PagedResponse<SubmissionResponse>>(paged);

            return Ok(ApiResponse<PagedResponse<SubmissionResponse>>.SuccessResponse(
                response, "Filtered submission list retrieved successfully"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SubmissionResponse>>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null)
                return NotFound(ApiResponse<SubmissionResponse>.FailResponse($"Submission with ID {id} not found.", 404));

            var response = _mapper.Map<SubmissionResponse>(item);
            return Ok(ApiResponse<SubmissionResponse>.SuccessResponse(response, "Submission retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SubmissionResponse>>> Create(SubmissionRequest request)
        {
            try
            {
                var created = await _service.CreateSubmissionAsync(_mapper.Map<SubmissionBM>(request));
                var response = _mapper.Map<SubmissionResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Submissionid },
                    ApiResponse<SubmissionResponse>.SuccessResponse(response, "Submission created successfully", 201)
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<SubmissionResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<SubmissionResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<SubmissionResponse>.FailResponse(ex.Message, 409));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<SubmissionResponse>.FailResponse(
                    "An unexpected error occurred while creating the submission.", 500));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<SubmissionResponse>>> Update(int id, SubmissionRequest request)
        {
            try
            {
                var updated = await _service.UpdateSubmissionAsync(id, _mapper.Map<SubmissionBM>(request));
                if (updated == null)
                    return NotFound(ApiResponse<SubmissionResponse>.FailResponse($"Submission with ID {id} not found.", 404));

                var response = _mapper.Map<SubmissionResponse>(updated);
                return Ok(ApiResponse<SubmissionResponse>.SuccessResponse(response, "Submission updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<SubmissionResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<SubmissionResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<SubmissionResponse>.FailResponse(ex.Message, 409));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<SubmissionResponse>.FailResponse(
                    "An unexpected error occurred while updating the submission.", 500));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            //var ok = await _service.DeleteAsync(id);
            //if (!ok)
            //    return NotFound(ApiResponse<string>.FailResponse($"Submission with ID {id} not found.", 404));

            return NoContent();
        }
    }
}
