using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRM.Application.Services;
using HRM.Api.Middleware;

namespace HRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HRManager")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    [HttpGet("headcount")]
    public async Task<ActionResult<HeadcountReportDto>> GetHeadcountReport([FromQuery] DateTime? date)
    {
        var branchId = BranchScopeMiddleware.GetBranchId(HttpContext);
        var report = await _reportsService.GetHeadcountReportAsync(branchId, date);
        return Ok(report);
    }

    [HttpGet("attrition")]
    public async Task<ActionResult<AttritionReportDto>> GetAttritionReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var branchId = BranchScopeMiddleware.GetBranchId(HttpContext);
        var report = await _reportsService.GetAttritionReportAsync(branchId, startDate, endDate);
        return Ok(report);
    }

    [HttpGet("leave-balances")]
    public async Task<ActionResult<List<LeaveBalanceReportDto>>> GetLeaveBalancesReport()
    {
        var branchId = BranchScopeMiddleware.GetBranchId(HttpContext);
        var report = await _reportsService.GetLeaveBalancesReportAsync(branchId);
        return Ok(report);
    }

    [HttpGet("absence")]
    public async Task<ActionResult<AbsenceReportDto>> GetAbsenceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var branchId = BranchScopeMiddleware.GetBranchId(HttpContext);
        var report = await _reportsService.GetAbsenceReportAsync(branchId, startDate, endDate);
        return Ok(report);
    }
}

