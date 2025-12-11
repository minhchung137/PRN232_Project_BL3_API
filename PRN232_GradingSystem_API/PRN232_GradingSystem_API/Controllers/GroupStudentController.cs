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
    public class GroupStudentController : ControllerBase
    {
        private readonly IGroupStudentService _service;
        private readonly IMapper _mapper;

        public GroupStudentController(IGroupStudentService service, IMapper mapper)
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
       

        // ===== GET: api/groupstudent/filter =====
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PagedResponse<GroupStudentResponse>>>> GetFilter([FromQuery] GroupStudentFilterRequest request)
        {
            var filter = _mapper.Map<GroupStudentBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
            var response = _mapper.Map<PagedResponse<GroupStudentResponse>>(paged);

            return Ok(ApiResponse<PagedResponse<GroupStudentResponse>>.SuccessResponse(
                response, "Filtered group student list retrieved successfully"));
        }

        // ===== GET: api/groupstudent/{groupId}/{studentId} =====
        [HttpGet("{groupId:int}/{studentId:int}")]
        public async Task<ActionResult<ApiResponse<GroupStudentResponse>>> GetById(int groupId, int studentId)
        {
            var item = await _service.GetByIdAsync(groupId, studentId);
            if (item == null)
                return NotFound(ApiResponse<GroupStudentResponse>.FailResponse("Group student record not found.", 404));

            var response = _mapper.Map<GroupStudentResponse>(item);
            return Ok(ApiResponse<GroupStudentResponse>.SuccessResponse(response, "Group student retrieved successfully"));
        }

        // ===== POST: api/groupstudent =====
        [HttpPost]
        public async Task<ActionResult<ApiResponse<GroupStudentResponse>>> Create(GroupStudentRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(_mapper.Map<GroupStudentBM>(request));
                var response = _mapper.Map<GroupStudentResponse>(created);

                return CreatedAtAction(
                    nameof(GetById),
                    new { groupId = response.Groupid, studentId = response.Studentid },
                    ApiResponse<GroupStudentResponse>.SuccessResponse(response, "Group student created successfully", 201)
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

        // ===== PUT: api/groupstudent/{groupId}/{studentId} =====
        //[HttpPut("{groupId:int}/{studentId:int}")]
        //public async Task<ActionResult<ApiResponse<GroupStudentResponse>>> Update(int groupId, int studentId, GroupStudentRequest request)
        //{
        //    try
        //    {
        //        var updated = await _service.UpdateAsync(groupId, studentId, _mapper.Map<GroupStudentBM>(request));
        //        if (updated == null)
        //            return NotFound(ApiResponse<GroupStudentResponse>.FailResponse($"Group student ({groupId}, {studentId}) not found.", 404));

        //        var response = _mapper.Map<GroupStudentResponse>(updated);
        //        return Ok(ApiResponse<GroupStudentResponse>.SuccessResponse(response, "Group student updated successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        //            return NotFound(ApiResponse<GroupStudentResponse>.FailResponse(ex.Message, 404));

        //        if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        //            return Conflict(ApiResponse<GroupStudentResponse>.FailResponse(ex.Message, 409));

        //        return StatusCode(500, ApiResponse<GroupStudentResponse>.FailResponse(
        //            "An unexpected error occurred while updating the group student.", 500));
        //    }
        //}

        // ===== DELETE: api/groupstudent/{groupId}/{studentId} =====
        [HttpDelete("{groupId:int}/{studentId:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int groupId, int studentId)
        {
            try
            {
                var ok = await _service.DeleteAsync(groupId, studentId);
                if (!ok)
                    return NotFound(ApiResponse<string>.FailResponse($"Group student ({groupId}, {studentId}) not found.", 404));

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
