namespace HRM.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    public string ActorId { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public Guid? BranchId { get; set; }
    public string? BeforeJson { get; set; } // JSON snapshot before change
    public string? AfterJson { get; set; } // JSON snapshot after change
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public string? IpAddress { get; set; }
}

