using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Infrastructure.Data;

namespace HRM.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await SeedBranches(context);
        await SeedRoles(context);
        await SeedUsers(context, userManager, roleManager);
        await SeedDepartments(context);
        await SeedPositions(context);
        await SeedLeaveTypes(context);
        
        await context.SaveChangesAsync();
    }

    private static async Task SeedBranches(ApplicationDbContext context)
    {
        if (context.Branches.Any()) return;

        var branches = new[]
        {
            new Branch { Name = "Head Office", Code = "HO", Address = "123 Main Street", IsActive = true },
            new Branch { Name = "Branch 1", Code = "BR1", Address = "456 Oak Avenue", IsActive = true },
            new Branch { Name = "Branch 2", Code = "BR2", Address = "789 Pine Road", IsActive = true }
        };

        await context.Branches.AddRangeAsync(branches);
    }

    private static async Task SeedRoles(ApplicationDbContext context)
    {
        if (context.HRMRoles.Any()) return;

        var roles = new[]
        {
            new Role { Name = "Admin", Description = "System Administrator", IsSystemRole = true },
            new Role { Name = "HR Manager", Description = "Human Resources Manager", IsSystemRole = true },
            new Role { Name = "Manager", Description = "Department Manager", IsSystemRole = true },
            new Role { Name = "Employee", Description = "Regular Employee", IsSystemRole = true }
        };

        await context.HRMRoles.AddRangeAsync(roles);
    }

    private static async Task SeedUsers(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (userManager.Users.Any()) return;

        // Create Identity roles
        var identityRoles = new[] { "Admin", "HRManager", "Manager", "Employee" };
        foreach (var roleName in identityRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create admin user
        var adminUser = new ApplicationUser
        {
            UserName = "admin@hrm.com",
            Email = "admin@hrm.com",
            FirstName = "Admin",
            LastName = "User",
            IsActive = true,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    private static async Task SeedDepartments(ApplicationDbContext context)
    {
        if (context.Departments.Any()) return;

        var headOffice = await context.Branches.FirstAsync(b => b.Code == "HO");
        
        var departments = new[]
        {
            new Department { Name = "Human Resources", Code = "HR", BranchId = headOffice.Id, IsActive = true },
            new Department { Name = "IT", Code = "IT", BranchId = headOffice.Id, IsActive = true },
            new Department { Name = "Finance", Code = "FIN", BranchId = headOffice.Id, IsActive = true },
            new Department { Name = "Operations", Code = "OPS", BranchId = headOffice.Id, IsActive = true }
        };

        await context.Departments.AddRangeAsync(departments);
    }

    private static async Task SeedPositions(ApplicationDbContext context)
    {
        if (context.Positions.Any()) return;

        var hrDept = await context.Departments.FirstAsync(d => d.Code == "HR");
        var itDept = await context.Departments.FirstAsync(d => d.Code == "IT");
        
        var positions = new[]
        {
            new Position { Title = "HR Manager", Code = "HRMGR", DepartmentId = hrDept.Id, Level = 5, IsActive = true },
            new Position { Title = "HR Specialist", Code = "HRSPC", DepartmentId = hrDept.Id, Level = 3, IsActive = true },
            new Position { Title = "IT Manager", Code = "ITMGR", DepartmentId = itDept.Id, Level = 5, IsActive = true },
            new Position { Title = "Software Developer", Code = "DEV", DepartmentId = itDept.Id, Level = 2, IsActive = true }
        };

        await context.Positions.AddRangeAsync(positions);
    }

    private static async Task SeedLeaveTypes(ApplicationDbContext context)
    {
        if (context.LeaveTypes.Any()) return;

        var leaveTypes = new[]
        {
            new LeaveType { Name = "Annual Leave", Code = "AL", MaxDays = 21, CarryForward = true, IsActive = true },
            new LeaveType { Name = "Sick Leave", Code = "SL", MaxDays = 10, CarryForward = false, IsActive = true },
            new LeaveType { Name = "Personal Leave", Code = "PL", MaxDays = 5, CarryForward = false, IsActive = true },
            new LeaveType { Name = "Maternity Leave", Code = "ML", MaxDays = 90, CarryForward = false, IsActive = true }
        };

        await context.LeaveTypes.AddRangeAsync(leaveTypes);
    }
}

