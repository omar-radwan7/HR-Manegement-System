using HRM.Application.DTOs;

namespace HRM.Application.Services;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetEmployeesAsync(Guid? branchId, string? searchTerm);
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id);
    Task<EmployeeDto> CreateEmployeeAsync(EmployeeCreateDto dto);
    Task<EmployeeDto> UpdateEmployeeAsync(Guid id, EmployeeUpdateDto dto);
    Task<bool> DeleteEmployeeAsync(Guid id);
}

