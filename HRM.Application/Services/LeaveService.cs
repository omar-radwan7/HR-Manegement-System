using Microsoft.EntityFrameworkCore;
using HRM.Application.DTOs;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly ApplicationDbContext _context;
    private readonly IApprovalEngine _approvalEngine;

    public LeaveService(ApplicationDbContext context, IApprovalEngine approvalEngine)
    {
        _context = context;
        _approvalEngine = approvalEngine;
    }

    public async Task<List<LeaveRequestDto>> GetLeaveRequestsAsync(Guid? branchId, Guid? employeeId)
    {
        var query = _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.Branch)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(lr => lr.BranchId == branchId.Value);

        if (employeeId.HasValue)
            query = query.Where(lr => lr.EmployeeId == employeeId.Value);

        var requests = await query.OrderByDescending(lr => lr.CreatedAt).ToListAsync();

        return requests.Select(lr => new LeaveRequestDto
        {
            Id = lr.Id,
            EmployeeId = lr.EmployeeId,
            EmployeeName = $"{lr.Employee.FirstName} {lr.Employee.LastName}",
            LeaveTypeId = lr.LeaveTypeId,
            LeaveTypeName = lr.LeaveType.Name,
            StartDate = lr.StartDate,
            EndDate = lr.EndDate,
            Days = lr.Days,
            Status = lr.Status,
            Reason = lr.Reason,
            BranchId = lr.BranchId,
            CreatedAt = lr.CreatedAt
        }).ToList();
    }

    public async Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id)
    {
        var request = await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.Branch)
            .FirstOrDefaultAsync(lr => lr.Id == id);

        if (request == null)
            return null;

        return new LeaveRequestDto
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            EmployeeName = $"{request.Employee.FirstName} {request.Employee.LastName}",
            LeaveTypeId = request.LeaveTypeId,
            LeaveTypeName = request.LeaveType.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Days = request.Days,
            Status = request.Status,
            Reason = request.Reason,
            BranchId = request.BranchId,
            CreatedAt = request.CreatedAt
        };
    }

    public async Task<LeaveRequestDto> CreateLeaveRequestAsync(LeaveRequestCreateDto dto)
    {
        // Validate leave request
        var employee = await _context.Employees.FindAsync(dto.EmployeeId);
        if (employee == null)
            throw new ArgumentException("Employee not found");

        var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
        if (leaveType == null)
            throw new ArgumentException("Leave type not found");

        // Check for overlapping requests
        var overlapping = await _context.LeaveRequests
            .AnyAsync(lr => lr.EmployeeId == dto.EmployeeId &&
                           lr.Status == LeaveRequestStatus.Pending &&
                           lr.StartDate <= dto.EndDate && lr.EndDate >= dto.StartDate);

        if (overlapping)
            throw new InvalidOperationException("Overlapping leave request exists");

        // Check balance
        var balance = await GetLeaveBalanceAsync(dto.EmployeeId, dto.LeaveTypeId);
        var days = (dto.EndDate - dto.StartDate).Days + 1;
        if (days > balance)
            throw new InvalidOperationException($"Insufficient leave balance. Available: {balance}, Requested: {days}");

        var request = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Days = days,
            Status = LeaveRequestStatus.Pending,
            Reason = dto.Reason,
            BranchId = employee.BranchId
        };

        _context.LeaveRequests.Add(request);
        await _context.SaveChangesAsync();

        // Resolve approvers and create workflow
        var approvers = await _approvalEngine.ResolveApproversAsync(
            "LeaveRequest", request.Id, employee.BranchId, employee.DepartmentId);

        // TODO: Create approval workflow and send notifications

        return await GetLeaveRequestByIdAsync(request.Id) ?? throw new InvalidOperationException("Failed to retrieve created leave request");
    }

    public async Task<bool> ApproveLeaveRequestAsync(Guid id, string approverId, bool approved, string? comments)
    {
        var request = await _context.LeaveRequests.FindAsync(id);
        if (request == null)
            return false;

        if (request.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Leave request is not pending");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (approved)
            {
                request.Status = LeaveRequestStatus.Approved;
                // TODO: Deduct from leave balance atomically
            }
            else
            {
                request.Status = LeaveRequestStatus.Rejected;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // TODO: Send notification to employee

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> GetLeaveBalanceAsync(Guid employeeId, Guid leaveTypeId)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
        if (leaveType == null)
            return 0;

        var usedDays = await _context.LeaveRequests
            .Where(lr => lr.EmployeeId == employeeId &&
                        lr.LeaveTypeId == leaveTypeId &&
                        lr.Status == LeaveRequestStatus.Approved)
            .SumAsync(lr => lr.Days);

        return Math.Max(0, leaveType.MaxDays - usedDays);
    }

    public async Task<LeaveImportResult> ImportLeaveRequestsAsync(Stream csvStream, bool dryRun)
    {
        var result = new LeaveImportResult();
        // TODO: Implement CSV import with validation
        await Task.CompletedTask;
        return result;
    }
}

