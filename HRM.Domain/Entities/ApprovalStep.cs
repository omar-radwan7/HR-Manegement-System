using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class ApprovalStep : BaseEntity
{
    public Guid ApprovalRuleId { get; set; }
    public int StepOrder { get; set; }
    public ApprovalStepType StepType { get; set; } = ApprovalStepType.Sequential;
    public Guid? RoleId { get; set; } // Approver role
    public string? UserId { get; set; } // Specific approver user ID
    public bool IsRequired { get; set; } = true;

    // Navigation properties
    public virtual ApprovalRule ApprovalRule { get; set; } = null!;
    public virtual Role? Role { get; set; }
}

