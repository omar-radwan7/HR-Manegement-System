using Microsoft.EntityFrameworkCore;
using HRM.Application.DTOs;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public class ReportsService : IReportsService
{
    private readonly ApplicationDbContext _context;

    public ReportsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HeadcountReportDto> GetHeadcountReportAsync(Guid? branchId, DateTime? date)
    {
        var reportDate = date ?? DateTime.UtcNow;
        var query = _context.Employees.Where(e => e.IsActive && e.HireDate <= reportDate);

        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var employees = await query
            .Include(e => e.Department)
            .Include(e => e.Position)
            .ToListAsync();

        var byDepartment = employees
            .GroupBy(e => e.Department.Name)
            .Select(g => new DepartmentHeadcount { DepartmentName = g.Key, Count = g.Count() })
            .ToList();

        var byPosition = employees
            .GroupBy(e => e.Position.Title)
            .Select(g => new PositionHeadcount { PositionTitle = g.Key, Count = g.Count() })
            .ToList();

        return new HeadcountReportDto
        {
            ReportDate = reportDate,
            ByDepartment = byDepartment,
            ByPosition = byPosition
        };
    }

    public async Task<AttritionReportDto> GetAttritionReportAsync(Guid? branchId, DateTime startDate, DateTime endDate)
    {
        var query = _context.Employees.Where(e => !e.IsActive && e.UpdatedAt >= startDate && e.UpdatedAt <= endDate);

        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var employees = await query
            .Include(e => e.Department)
            .ToListAsync();

        var byDepartment = employees
            .GroupBy(e => e.Department.Name)
            .Select(g => new AttritionByDepartment { DepartmentName = g.Key, Count = g.Count() })
            .ToList();

        return new AttritionReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalAttrition = employees.Count,
            ByDepartment = byDepartment
        };
    }

    public async Task<List<LeaveBalanceReportDto>> GetLeaveBalancesReportAsync(Guid? branchId)
    {
        var query = _context.Employees.Where(e => e.IsActive);

        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var employees = await query
            .Include(e => e.LeaveRequests)
            .ThenInclude(lr => lr.LeaveType)
            .ToListAsync();

        var leaveTypes = await _context.LeaveTypes.ToListAsync();
        var result = new List<LeaveBalanceReportDto>();

        foreach (var employee in employees)
        {
            foreach (var leaveType in leaveTypes)
            {
                var usedDays = employee.LeaveRequests
                    .Where(lr => lr.LeaveTypeId == leaveType.Id && lr.Status == Domain.Enums.LeaveRequestStatus.Approved)
                    .Sum(lr => lr.Days);

                var balance = Math.Max(0, leaveType.MaxDays - usedDays);

                result.Add(new LeaveBalanceReportDto
                {
                    EmployeeId = employee.Id,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    LeaveTypeName = leaveType.Name,
                    Balance = balance
                });
            }
        }

        return result;
    }

    public async Task<AbsenceReportDto> GetAbsenceReportAsync(Guid? branchId, DateTime startDate, DateTime endDate)
    {
        var query = _context.AttendanceRecords
            .Where(ar => ar.Date >= startDate && ar.Date <= endDate && ar.Status == Domain.Enums.AttendanceStatus.Absent);

        if (branchId.HasValue)
            query = query.Where(ar => ar.BranchId == branchId.Value);

        var records = await query
            .Include(ar => ar.Employee)
            .ToListAsync();

        var byEmployee = records
            .GroupBy(ar => new { ar.EmployeeId, ar.Employee.FirstName, ar.Employee.LastName })
            .Select(g => new AbsenceByEmployee
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                AbsenceDays = g.Count()
            })
            .ToList();

        return new AbsenceReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ByEmployee = byEmployee
        };
    }
}

