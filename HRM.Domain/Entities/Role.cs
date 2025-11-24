namespace HRM.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = false;

    // Navigation properties
    public virtual ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
    public virtual ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();
}

