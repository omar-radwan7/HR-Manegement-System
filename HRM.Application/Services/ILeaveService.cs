using HRM.Application.DTOs;

namespace HRM.Application.Services;

public interface ILeaveService
{
    Task<List<LeaveRequestDto>> GetLeaveRequestsAsync(Guid? branchId, Guid? employeeId);
    Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id);
    Task<LeaveRequestDto> CreateLeaveRequestAsync(LeaveRequestCreateDto dto);
    Task<bool> ApproveLeaveRequestAsync(Guid id, string approverId, bool approved, string? comments);
    Task<int> GetLeaveBalanceAsync(Guid employeeId, Guid leaveTypeId);
    Task<LeaveImportResult> ImportLeaveRequestsAsync(Stream csvStream, bool dryRun);
}

public class LeaveImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

