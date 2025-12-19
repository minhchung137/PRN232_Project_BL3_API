using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public UserController(IUserService service, IMapper mapper)
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
        // GET: api/user/filter
        [HttpGet("filter")]
        public async Task<ActionResult<PagedResponse<UserResponse>>> GetFilter([FromQuery] UserRequest request)
        {
            var filter = _mapper.Map<UserBM>(request);
            var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
            return Ok(_mapper.Map<PagedResponse<UserResponse>>(paged));
        }

        // GET: api/user/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponse>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id, includeDetails: true);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<UserResponse>(item));
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create(UserRequest request)
        {
            var created = await _service.CreateAsync(_mapper.Map<UserBM>(request));
            var response = _mapper.Map<UserResponse>(created);
            return CreatedAtAction(nameof(GetById), new { id = response.UserId }, response);
        }

        // PUT: api/user/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponse>> Update(int id, UserRequest request)
        {
            var updated = await _service.UpdateAsync(id, _mapper.Map<UserBM>(request));
            if (updated == null) return NotFound();
            return Ok(_mapper.Map<UserResponse>(updated));
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
