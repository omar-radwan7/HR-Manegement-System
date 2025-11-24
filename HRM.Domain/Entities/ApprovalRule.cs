namespace HRM.Domain.Entities;

public class ApprovalRule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // e.g., "LeaveRequest", "Document"
    public Guid? BranchId { get; set; } // null means global rule
    public Guid? DepartmentId { get; set; } // null means branch-level or global
    public int Priority { get; set; } // Higher priority = more specific (evaluated first)
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Branch? Branch { get; set; }
    public virtual Department? Department { get; set; }
    public virtual ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
}

