using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public Guid BranchId { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}

