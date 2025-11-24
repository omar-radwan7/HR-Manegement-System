using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRM.Application.Services;
using HRM.Api.Middleware;

namespace HRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    {
        var branchId = BranchScopeMiddleware.GetBranchId(HttpContext);
        if (!branchId.HasValue)
            return BadRequest("Branch ID is required");

        var result = await _attendanceService.CheckInAsync(request.EmployeeId, branchId.Value);
        if (!result)
            return BadRequest("Already checked in today");

        return Ok();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
    {
        var branchId = BranchScopeMiddleware.GetBranchId(HttpContext);
        if (!branchId.HasValue)
            return BadRequest("Branch ID is required");

        var result = await _attendanceService.CheckOutAsync(request.EmployeeId, branchId.Value);
        if (!result)
            return BadRequest("Not checked in or already checked out");

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult> GetAttendanceRecords([FromQuery] Guid? employeeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var records = await _attendanceService.GetAttendanceRecordsAsync(employeeId, startDate, endDate);
        return Ok(records);
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<AttendanceImportResult>> ImportAttendance(IFormFile file, [FromQuery] bool dryRun = true)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        var result = await _attendanceService.ImportAttendanceAsync(stream, dryRun);
        return Ok(result);
    }
}

public class CheckInRequest
{
    public Guid EmployeeId { get; set; }
}

public class CheckOutRequest
{
    public Guid EmployeeId { get; set; }
}

