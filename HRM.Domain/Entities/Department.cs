namespace HRM.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public Guid? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Branch Branch { get; set; } = null!;
    public virtual Employee? Manager { get; set; }
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
    public virtual ICollection<ApprovalRule> ApprovalRules { get; set; } = new List<ApprovalRule>();
}

