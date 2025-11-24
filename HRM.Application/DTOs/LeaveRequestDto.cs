using HRM.Domain.Enums;

namespace HRM.Application.DTOs;

public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public string? Reason { get; set; }
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LeaveRequestCreateDto
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
}

public class LeaveRequestApproveDto
{
    public bool Approved { get; set; }
    public string? Comments { get; set; }
}

