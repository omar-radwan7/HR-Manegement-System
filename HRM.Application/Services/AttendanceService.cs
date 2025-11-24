using Microsoft.EntityFrameworkCore;
using HRM.Application.DTOs;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;

    public AttendanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckInAsync(Guid employeeId, Guid branchId)
    {
        var today = DateTime.UtcNow.Date;
        
        // Check if already checked in today
        var existing = await _context.AttendanceRecords
            .FirstOrDefaultAsync(ar => ar.EmployeeId == employeeId && ar.Date == today);

        if (existing != null && existing.CheckInTime.HasValue)
            return false; // Already checked in

        if (existing == null)
        {
            existing = new AttendanceRecord
            {
                EmployeeId = employeeId,
                BranchId = branchId,
                Date = today,
                CheckInTime = DateTime.UtcNow,
                Status = AttendanceStatus.Present
            };
            _context.AttendanceRecords.Add(existing);
        }
        else
        {
            existing.CheckInTime = DateTime.UtcNow;
            existing.Status = AttendanceStatus.Present;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckOutAsync(Guid employeeId, Guid branchId)
    {
        var today = DateTime.UtcNow.Date;
        
        var record = await _context.AttendanceRecords
            .FirstOrDefaultAsync(ar => ar.EmployeeId == employeeId && ar.Date == today);

        if (record == null || !record.CheckInTime.HasValue)
            return false; // Not checked in

        if (record.CheckOutTime.HasValue)
            return false; // Already checked out

        record.CheckOutTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AttendanceRecordDto>> GetAttendanceRecordsAsync(Guid? employeeId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.AttendanceRecords
            .Include(ar => ar.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(ar => ar.EmployeeId == employeeId.Value);

        if (startDate.HasValue)
            query = query.Where(ar => ar.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ar => ar.Date <= endDate.Value);

        var records = await query.OrderByDescending(ar => ar.Date).ToListAsync();

        return records.Select(ar => new AttendanceRecordDto
        {
            Id = ar.Id,
            EmployeeId = ar.EmployeeId,
            EmployeeName = $"{ar.Employee.FirstName} {ar.Employee.LastName}",
            Date = ar.Date,
            CheckInTime = ar.CheckInTime,
            CheckOutTime = ar.CheckOutTime,
            Status = ar.Status.ToString()
        }).ToList();
    }

    public async Task<AttendanceImportResult> ImportAttendanceAsync(Stream csvStream, bool dryRun)
    {
        var result = new AttendanceImportResult();
        // TODO: Implement CSV import with duplicate prevention
        await Task.CompletedTask;
        return result;
    }
}

