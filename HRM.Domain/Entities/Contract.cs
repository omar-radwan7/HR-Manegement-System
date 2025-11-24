using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class Contract : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public ContractType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Salary { get; set; }
    public ContractStatus Status { get; set; } = ContractStatus.Active;

    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
}

