using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HRM.Domain.Entities;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private static readonly HashSet<string> SensitiveFields = new()
    {
        "Salary", "salary", "DateOfBirth", "dateOfBirth", "Phone", "phone", "Email", "email"
    };

    public AuthorizationService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> CanReadAsync<T>(T entity, ApplicationUser user, Guid? branchId) where T : class
    {
        if (await IsAdminAsync(user))
            return true;

        // Check branch access
        if (branchId.HasValue && !await HasBranchAccessAsync(user, branchId.Value))
            return false;

        // Entity-specific read permissions
        return entity switch
        {
            Employee emp => await CanReadEmployeeAsync(emp, user, branchId),
            LeaveRequest lr => await CanReadLeaveRequestAsync(lr, user, branchId),
            Document doc => await CanReadDocumentAsync(doc, user, branchId),
            _ => true // Default allow for other entities
        };
    }

    public async Task<bool> CanWriteAsync<T>(T entity, ApplicationUser user, Guid? branchId) where T : class
    {
        if (await IsAdminAsync(user))
            return true;

        if (branchId.HasValue && !await HasBranchAccessAsync(user, branchId.Value))
            return false;

        return entity switch
        {
            Employee emp => await CanWriteEmployeeAsync(emp, user, branchId),
            LeaveRequest lr => await CanWriteLeaveRequestAsync(lr, user, branchId),
            Document doc => await CanWriteDocumentAsync(doc, user, branchId),
            _ => await IsHRManagerAsync(user) || await IsManagerAsync(user)
        };
    }

    public async Task<bool> CanApproveAsync<T>(T entity, ApplicationUser user, Guid? branchId) where T : class
    {
        if (await IsAdminAsync(user))
            return true;

        if (branchId.HasValue && !await HasBranchAccessAsync(user, branchId.Value))
            return false;

        return entity switch
        {
            LeaveRequest lr => await CanApproveLeaveRequestAsync(lr, user),
            _ => await IsHRManagerAsync(user) || await IsManagerAsync(user)
        };
    }

    public bool ShouldMaskField(string fieldName, ApplicationUser user)
    {
        if (!SensitiveFields.Contains(fieldName))
            return false;

        // Admin and HR can see all fields
        if (user == null)
            return true;

        var roles = _userManager.GetRolesAsync(user).Result;
        if (roles.Contains("Admin") || roles.Contains("HRManager"))
            return false;

        return true;
    }

    private async Task<bool> IsAdminAsync(ApplicationUser user)
    {
        return await _userManager.IsInRoleAsync(user, "Admin");
    }

    private async Task<bool> IsHRManagerAsync(ApplicationUser user)
    {
        return await _userManager.IsInRoleAsync(user, "HRManager");
    }

    private async Task<bool> IsManagerAsync(ApplicationUser user)
    {
        return await _userManager.IsInRoleAsync(user, "Manager");
    }

    private async Task<bool> HasBranchAccessAsync(ApplicationUser user, Guid branchId)
    {
        // Check if user has global access (no branch restriction) or specific branch access
        var roleAssignments = await _context.RoleAssignments
            .Where(ra => ra.UserId == user.Id)
            .ToListAsync();

        // Global access (BranchId is null)
        if (roleAssignments.Any(ra => ra.BranchId == null))
            return true;

        // Branch-specific access
        return roleAssignments.Any(ra => ra.BranchId == branchId);
    }

    private async Task<bool> CanReadEmployeeAsync(Employee employee, ApplicationUser user, Guid? branchId)
    {
        // Users can read their own employee record
        if (employee.Email == user.Email)
            return true;

        return await IsHRManagerAsync(user) || await IsManagerAsync(user);
    }

    private async Task<bool> CanWriteEmployeeAsync(Employee employee, ApplicationUser user, Guid? branchId)
    {
        return await IsHRManagerAsync(user);
    }

    private async Task<bool> CanReadLeaveRequestAsync(LeaveRequest leaveRequest, ApplicationUser user, Guid? branchId)
    {
        // Users can read their own leave requests
        var employee = await _context.Employees.FindAsync(leaveRequest.EmployeeId);
        if (employee?.Email == user.Email)
            return true;

        return await IsHRManagerAsync(user) || await IsManagerAsync(user);
    }

    private async Task<bool> CanWriteLeaveRequestAsync(LeaveRequest leaveRequest, ApplicationUser user, Guid? branchId)
    {
        // Users can create their own leave requests
        var employee = await _context.Employees.FindAsync(leaveRequest.EmployeeId);
        if (employee?.Email == user.Email)
            return true;

        return await IsHRManagerAsync(user);
    }

    private async Task<bool> CanApproveLeaveRequestAsync(LeaveRequest leaveRequest, ApplicationUser user)
    {
        return await IsHRManagerAsync(user) || await IsManagerAsync(user);
    }

    private async Task<bool> CanReadDocumentAsync(Document document, ApplicationUser user, Guid? branchId)
    {
        var employee = await _context.Employees.FindAsync(document.EmployeeId);
        if (employee?.Email == user.Email)
            return true;

        return await IsHRManagerAsync(user) || await IsManagerAsync(user);
    }

    private async Task<bool> CanWriteDocumentAsync(Document document, ApplicationUser user, Guid? branchId)
    {
        return await IsHRManagerAsync(user);
    }
}

