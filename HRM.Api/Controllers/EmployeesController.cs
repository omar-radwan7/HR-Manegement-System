using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRM.Application.DTOs;
using HRM.Application.Services;
using HRM.Api.Middleware;
using HRM.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using AppAuthService = HRM.Application.Services.IAuthorizationService;

namespace HRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly AppAuthService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public EmployeesController(
        IEmployeeService employeeService,
        AppAuthService authorizationService,
        UserManager<ApplicationUser> userManager)
    {
        _employeeService = employeeService;
        _authorizationService = authorizationService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployees([FromQuery] string? searchTerm)
    {
        var branchId = BranchScopeMiddleware.GetBranchId(HttpContext);
        var employees = await _employeeService.GetEmployeesAsync(branchId, searchTerm);

        // Apply field masking based on user permissions
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            foreach (var employee in employees)
            {
                if (_authorizationService.ShouldMaskField("Salary", user))
                    employee.Salary = null;
                if (_authorizationService.ShouldMaskField("DateOfBirth", user))
                    employee.DateOfBirth = null;
                if (_authorizationService.ShouldMaskField("Phone", user))
                    employee.Phone = null;
            }
        }

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        // Apply field masking
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            if (_authorizationService.ShouldMaskField("Salary", user))
                employee.Salary = null;
            if (_authorizationService.ShouldMaskField("DateOfBirth", user))
                employee.DateOfBirth = null;
            if (_authorizationService.ShouldMaskField("Phone", user))
                employee.Phone = null;
        }

        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeCreateDto dto)
    {
        var employee = await _employeeService.CreateEmployeeAsync(dto);
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(Guid id, EmployeeUpdateDto dto)
    {
        try
        {
            var employee = await _employeeService.UpdateEmployeeAsync(id, dto);
            return Ok(employee);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        var result = await _employeeService.DeleteEmployeeAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

