namespace HRM.Domain.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int MaxDays { get; set; }
    public bool CarryForward { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}

