using Microsoft.EntityFrameworkCore;
using HRM.Application.DTOs;
using HRM.Domain.Entities;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;

    public EmployeeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync(Guid? branchId, string? searchTerm)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Branch)
            .AsQueryable();

        if (branchId.HasValue)
        {
            query = query.Where(e => e.BranchId == branchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(e =>
                e.FirstName.ToLower().Contains(searchTerm) ||
                e.LastName.ToLower().Contains(searchTerm) ||
                e.Email.ToLower().Contains(searchTerm) ||
                e.EmployeeNumber.ToLower().Contains(searchTerm));
        }

        var employees = await query.ToListAsync();

        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            EmployeeNumber = e.EmployeeNumber,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Phone = e.Phone,
            DateOfBirth = e.DateOfBirth,
            HireDate = e.HireDate,
            Salary = e.Salary,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department.Name,
            PositionId = e.PositionId,
            PositionTitle = e.Position.Title,
            BranchId = e.BranchId,
            BranchName = e.Branch.Name,
            IsActive = e.IsActive
        }).ToList();
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Branch)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return null;

        return new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            DateOfBirth = employee.DateOfBirth,
            HireDate = employee.HireDate,
            Salary = employee.Salary,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,
            PositionId = employee.PositionId,
            PositionTitle = employee.Position.Title,
            BranchId = employee.BranchId,
            BranchName = employee.Branch.Name,
            IsActive = employee.IsActive
        };
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeCreateDto dto)
    {
        var employee = new Employee
        {
            EmployeeNumber = dto.EmployeeNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            HireDate = dto.HireDate,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId,
            BranchId = dto.BranchId,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return await GetEmployeeByIdAsync(employee.Id) ?? throw new InvalidOperationException("Failed to retrieve created employee");
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, EmployeeUpdateDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found");

        if (dto.Phone != null)
            employee.Phone = dto.Phone;
        if (dto.Salary.HasValue)
            employee.Salary = dto.Salary.Value;
        if (dto.DepartmentId.HasValue)
            employee.DepartmentId = dto.DepartmentId.Value;
        if (dto.PositionId.HasValue)
            employee.PositionId = dto.PositionId.Value;
        if (dto.IsActive.HasValue)
            employee.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();

        return await GetEmployeeByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated employee");
    }

    public async Task<bool> DeleteEmployeeAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return false;

        // Soft delete
        employee.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}

