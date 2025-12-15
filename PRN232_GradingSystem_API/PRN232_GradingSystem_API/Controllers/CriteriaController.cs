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
    [Route("api/criteria")]
    public class CriteriaController : ControllerBase
    {
        private readonly ICriteriaService _service;
        private readonly IMapper _mapper;

        public CriteriaController(ICriteriaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CriteriaResponse>>> Create(
            [FromBody] CriteriaRequest req)
        {
            try
            {
                var bm = _mapper.Map<CriteriaBM>(req);
                var created = await _service.CreateAsync(bm);
                var response = _mapper.Map<CriteriaResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { criteriaId = response.Criteriaid },
                    ApiResponse<CriteriaResponse>.SuccessResponse(
                        response, "Criteria created successfully", 201)
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<CriteriaResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<CriteriaResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<CriteriaResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<CriteriaResponse>.FailResponse(
                        "An unexpected error occurred while creating criteria.", 500));
            }
        }
        
        [Authorize]
        [HttpGet("by-question/{questionId:int}")]
        public async Task<ActionResult<ApiResponse<List<CriteriaResponse>>>> GetByQuestion(
            int questionId)
        {
            try
            {
                var list = await _service.GetByQuestionAsync(questionId);
                var response = _mapper.Map<List<CriteriaResponse>>(list);

                return Ok(ApiResponse<List<CriteriaResponse>>.SuccessResponse(
                    response, "Criteria retrieved successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<List<CriteriaResponse>>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<List<CriteriaResponse>>.FailResponse(ex.Message, 404));
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<List<CriteriaResponse>>.FailResponse(
                        "An unexpected error occurred while retrieving criteria.", 500));
            }
        }
        
        [HttpGet("{criteriaId:int}")]
        public async Task<ActionResult<ApiResponse<CriteriaResponse>>> GetById(int criteriaId)
        {
            var item = await _service.GetByIdAsync(criteriaId);

            if (item == null)
            {
                return NotFound(ApiResponse<CriteriaResponse>.FailResponse(
                    "Criteria not found", 404));
            }

            var response = _mapper.Map<CriteriaResponse>(item);
            return Ok(ApiResponse<CriteriaResponse>.SuccessResponse(
                response, "Criteria retrieved successfully"));
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{criteriaId:int}")]
        public async Task<ActionResult<ApiResponse<CriteriaResponse>>> Update(
            int criteriaId,
            [FromBody] CriteriaUpdateRequest req)
        {
            try
            {
                var bm = _mapper.Map<CriteriaBM>(req);
                var ok = await _service.UpdateAsync(criteriaId, bm);

                if (!ok)
                {
                    return NotFound(ApiResponse<CriteriaResponse>.FailResponse(
                        $"Criteria with ID {criteriaId} not found.", 404));
                }

                var updated = await _service.GetByIdAsync(criteriaId);
                var response = _mapper.Map<CriteriaResponse>(updated);

                return Ok(ApiResponse<CriteriaResponse>.SuccessResponse(
                    response, "Criteria updated successfully"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<CriteriaResponse>.FailResponse(ex.Message, 400));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<CriteriaResponse>.FailResponse(ex.Message, 404));
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<CriteriaResponse>.FailResponse(ex.Message, 409));
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<CriteriaResponse>.FailResponse(
                        "An unexpected error occurred while updating criteria.", 500));
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{criteriaId:int}")]
        public async Task<ActionResult<ApiResponse<CriteriaResponse>>> Delete(int criteriaId)
        {
            try
            {
                var ok = await _service.DeleteAsync(criteriaId);

                if (!ok)
                {
                    return NotFound(ApiResponse<CriteriaResponse>.FailResponse(
                        $"Criteria with ID {criteriaId} not found.", 404));
                }
                
                return NoContent();
            }
            catch
            {
                return StatusCode(500,
                    ApiResponse<CriteriaResponse>.FailResponse(
                        "An unexpected error occurred while deleting criteria.", 500));
            }
        }
    }
}