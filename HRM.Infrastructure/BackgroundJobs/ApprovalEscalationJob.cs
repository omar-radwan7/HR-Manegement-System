using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Infrastructure.Data;

namespace HRM.Infrastructure.BackgroundJobs;

public class ApprovalEscalationJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApprovalEscalationJob> _logger;

    public ApprovalEscalationJob(ApplicationDbContext context, ILogger<ApprovalEscalationJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessEscalations()
    {
        _logger.LogInformation("Starting approval escalation job at {Time}", DateTime.UtcNow);

        // Find leave requests pending approval for more than SLA threshold (e.g., 2 days)
        var slaThreshold = DateTime.UtcNow.AddDays(-2);
        
        var pendingRequests = await _context.LeaveRequests
            .Where(lr => lr.Status == LeaveRequestStatus.Pending && lr.CreatedAt < slaThreshold)
            .ToListAsync();

        foreach (var request in pendingRequests)
        {
            try
            {
                // TODO: Implement escalation logic
                // 1. Find the next approver in the chain
                // 2. Send escalation notification
                // 3. Update audit log
                
                _logger.LogWarning("Escalating leave request {RequestId} for employee {EmployeeId}", 
                    request.Id, request.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error escalating leave request {RequestId}", request.Id);
            }
        }

        _logger.LogInformation("Completed approval escalation job. Processed {Count} requests", pendingRequests.Count);
    }
}

