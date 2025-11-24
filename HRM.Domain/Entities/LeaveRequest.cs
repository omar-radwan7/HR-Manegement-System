using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
    public string? Reason { get; set; }
    public Guid BranchId { get; set; }

    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual LeaveType LeaveType { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}

