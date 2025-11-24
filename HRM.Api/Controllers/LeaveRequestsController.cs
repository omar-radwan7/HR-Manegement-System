using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRM.Application.DTOs;
using HRM.Application.Services;

namespace HRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveRequestsController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetLeaveRequests([FromQuery] Guid? employeeId)
    {
        var branchId = HRM.Api.Middleware.BranchScopeMiddleware.GetBranchId(HttpContext);
        var requests = await _leaveService.GetLeaveRequestsAsync(branchId, employeeId);
        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequest(Guid id)
    {
        var request = await _leaveService.GetLeaveRequestByIdAsync(id);
        if (request == null)
            return NotFound();

        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(LeaveRequestCreateDto dto)
    {
        try
        {
            var request = await _leaveService.CreateLeaveRequestAsync(dto);
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = request.Id }, request);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,HRManager,Manager")]
    public async Task<IActionResult> ApproveLeaveRequest(Guid id, LeaveRequestApproveDto dto)
    {
        var userId = User.Identity?.Name ?? string.Empty;
        var result = await _leaveService.ApproveLeaveRequestAsync(id, userId, dto.Approved, dto.Comments);
        
        if (!result)
            return NotFound();

        return Ok();
    }

    [HttpGet("balance/{employeeId}/{leaveTypeId}")]
    public async Task<ActionResult<int>> GetLeaveBalance(Guid employeeId, Guid leaveTypeId)
    {
        var balance = await _leaveService.GetLeaveBalanceAsync(employeeId, leaveTypeId);
        return Ok(balance);
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<LeaveImportResult>> ImportLeaveRequests(IFormFile file, [FromQuery] bool dryRun = true)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        var result = await _leaveService.ImportLeaveRequestsAsync(stream, dryRun);
        return Ok(result);
    }
}

