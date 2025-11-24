namespace HRM.Domain.Entities;

public class RoleAssignment : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Guid? BranchId { get; set; } // null means global access
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string AssignedBy { get; set; } = string.Empty;

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Branch? Branch { get; set; }
}

