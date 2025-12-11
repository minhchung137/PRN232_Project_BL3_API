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
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _service;
        private readonly IMapper _mapper;

        public GroupController(IGroupService service, IMapper mapper)
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
        // GET: api/group/filter
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<GroupResponse>>>> GetFilter([FromQuery] GroupFilterRequest request)
        {
            var filter = _mapper.Map<GroupBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);

            var response = _mapper.Map<PagedResponse<GroupResponse>>(paged);
            return Ok(ApiResponse<PagedResponse<GroupResponse>>.SuccessResponse(response, "Filtered list retrieved successfully"));
        }

        // GET: api/group/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<GroupResponse>>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null)
                return NotFound(ApiResponse<GroupResponse>.FailResponse("Group not found", 404));

            var response = _mapper.Map<GroupResponse>(item);
            return Ok(ApiResponse<GroupResponse>.SuccessResponse(response, "Group retrieved successfully"));
        }

        // POST: api/group
        [HttpPost]
        public async Task<ActionResult<ApiResponse<GroupResponse>>> Create(GroupCreateRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<GroupBM>(request));
                var response = _mapper.Map<GroupResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Groupid },
                    ApiResponse<GroupResponse>.SuccessResponse(response, "Group created successfully", 201)
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

        // PUT: api/group/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<GroupResponse>>> Update(int id, GroupUpdateRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, _mapper.Map<GroupBM>(request));
                if (updated == null)
                    return NotFound(ApiResponse<GroupResponse>.FailResponse($"Group with ID {id} not found.", 404));

                var response = _mapper.Map<GroupResponse>(updated);
                return Ok(ApiResponse<GroupResponse>.SuccessResponse(response, "Group updated successfully"));
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


        //// DELETE: api/group/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                //var ok = await _service.DeleteAsync(id);
                //if (!ok)
                //    return NotFound(ApiResponse<object>.FailResponse($"Group with ID {id} not found.", 404));

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "An unexpected error occurred while deleting group.", 500));
            }
        }

    }
}
