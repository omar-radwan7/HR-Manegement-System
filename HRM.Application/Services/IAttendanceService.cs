using HRM.Application.DTOs;

namespace HRM.Application.Services;

public interface IAttendanceService
{
    Task<bool> CheckInAsync(Guid employeeId, Guid branchId);
    Task<bool> CheckOutAsync(Guid employeeId, Guid branchId);
    Task<List<AttendanceRecordDto>> GetAttendanceRecordsAsync(Guid? employeeId, DateTime? startDate, DateTime? endDate);
    Task<AttendanceImportResult> ImportAttendanceAsync(Stream csvStream, bool dryRun);
}

public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AttendanceImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

