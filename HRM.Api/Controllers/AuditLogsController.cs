using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRM.Infrastructure.Data;

namespace HRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HRManager")]
public class AuditLogsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuditLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAuditLogs(
        [FromQuery] string? entityType,
        [FromQuery] Guid? entityId,
        [FromQuery] string? actorId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(al => al.EntityType == entityType);

        if (entityId.HasValue)
            query = query.Where(al => al.EntityId == entityId.Value);

        if (!string.IsNullOrEmpty(actorId))
            query = query.Where(al => al.ActorId == actorId);

        if (startDate.HasValue)
            query = query.Where(al => al.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(al => al.Timestamp <= endDate.Value);

        var logs = await query.OrderByDescending(al => al.Timestamp).Take(1000).ToListAsync();
        return Ok(logs);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportAuditLogs([FromQuery] string? format = "json")
    {
        var logs = await _context.AuditLogs.OrderByDescending(al => al.Timestamp).Take(10000).ToListAsync();

        if (format?.ToLower() == "csv")
        {
            // TODO: Implement CSV export
            return Ok(logs);
        }

        return Ok(logs);
    }
}

