using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using HRM.Domain.Entities;
using System.Text.Json;

namespace HRM.Infrastructure.Data;

public class AuditLogInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SaveChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SaveChanges(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SaveChanges(DbContext? context)
    {
        if (context == null) return;

        var httpContext = _httpContextAccessor.HttpContext;
        var actorId = httpContext?.User?.Identity?.Name ?? "System";
        var actorName = httpContext?.User?.Identity?.Name ?? "System";
        var branchId = httpContext?.Items["BranchId"] as Guid?;
        var traceId = httpContext?.TraceIdentifier;
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

        var entries = context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var auditLog = new AuditLog
            {
                EntityType = entry.Entity.GetType().Name,
                EntityId = entry.Entity.Id,
                Action = entry.State.ToString(),
                ActorId = actorId,
                ActorName = actorName,
                BranchId = branchId,
                TraceId = traceId,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
            {
                var originalValues = entry.OriginalValues.Properties
                    .ToDictionary(p => p.Name, p => entry.OriginalValues[p]);
                auditLog.BeforeJson = JsonSerializer.Serialize(originalValues);
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var currentValues = entry.CurrentValues.Properties
                    .ToDictionary(p => p.Name, p => entry.CurrentValues[p]);
                auditLog.AfterJson = JsonSerializer.Serialize(currentValues);
            }

            context.Set<AuditLog>().Add(auditLog);
        }
    }
}

