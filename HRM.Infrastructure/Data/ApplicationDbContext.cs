using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using System.Text.Json;

namespace HRM.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Role> HRMRoles { get; set; }
    public DbSet<RoleAssignment> RoleAssignments { get; set; }
    public DbSet<ApprovalRule> ApprovalRules { get; set; }
    public DbSet<ApprovalStep> ApprovalSteps { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure enums
        builder.Entity<Contract>()
            .Property(e => e.Type)
            .HasConversion<int>();

        builder.Entity<Contract>()
            .Property(e => e.Status)
            .HasConversion<int>();

        builder.Entity<LeaveRequest>()
            .Property(e => e.Status)
            .HasConversion<int>();

        builder.Entity<AttendanceRecord>()
            .Property(e => e.Status)
            .HasConversion<int>();

        builder.Entity<ApprovalStep>()
            .Property(e => e.StepType)
            .HasConversion<int>();

        builder.Entity<Notification>()
            .Property(e => e.Type)
            .HasConversion<int>();

        // Configure relationships and indexes
        ConfigureEmployee(builder);
        ConfigureDepartment(builder);
        ConfigurePosition(builder);
        ConfigureContract(builder);
        ConfigureBranch(builder);
        ConfigureLeaveRequest(builder);
        ConfigureAttendanceRecord(builder);
        ConfigureDocument(builder);
        ConfigureRole(builder);
        ConfigureRoleAssignment(builder);
        ConfigureApprovalRule(builder);
        ConfigureApprovalStep(builder);
        ConfigureAuditLog(builder);
        ConfigureSetting(builder);
        ConfigureNotification(builder);
    }

    private void ConfigureEmployee(ModelBuilder builder)
    {
        builder.Entity<Employee>()
            .HasIndex(e => e.EmployeeNumber)
            .IsUnique();

        builder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        builder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Branch)
            .WithMany(b => b.Employees)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Contract)
            .WithMany()
            .HasForeignKey(e => e.ContractId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigureDepartment(ModelBuilder builder)
    {
        builder.Entity<Department>()
            .HasIndex(d => new { d.Code, d.BranchId })
            .IsUnique();

        builder.Entity<Department>()
            .HasOne(d => d.Branch)
            .WithMany(b => b.Departments)
            .HasForeignKey(d => d.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Department>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigurePosition(ModelBuilder builder)
    {
        builder.Entity<Position>()
            .HasIndex(p => new { p.Code, p.DepartmentId })
            .IsUnique();

        builder.Entity<Position>()
            .HasOne(p => p.Department)
            .WithMany(d => d.Positions)
            .HasForeignKey(p => p.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureContract(ModelBuilder builder)
    {
        builder.Entity<Contract>()
            .HasOne(c => c.Employee)
            .WithMany(e => e.Contracts)
            .HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureBranch(ModelBuilder builder)
    {
        builder.Entity<Branch>()
            .HasIndex(b => b.Code)
            .IsUnique();
    }

    private void ConfigureLeaveRequest(ModelBuilder builder)
    {
        builder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(lr => lr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveRequest>()
            .HasOne(lr => lr.LeaveType)
            .WithMany(lt => lt.LeaveRequests)
            .HasForeignKey(lr => lr.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Branch)
            .WithMany(b => b.LeaveRequests)
            .HasForeignKey(lr => lr.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveRequest>()
            .HasIndex(lr => new { lr.EmployeeId, lr.StartDate, lr.EndDate });
    }

    private void ConfigureAttendanceRecord(ModelBuilder builder)
    {
        builder.Entity<AttendanceRecord>()
            .HasOne(ar => ar.Employee)
            .WithMany(e => e.AttendanceRecords)
            .HasForeignKey(ar => ar.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AttendanceRecord>()
            .HasOne(ar => ar.Branch)
            .WithMany(b => b.AttendanceRecords)
            .HasForeignKey(ar => ar.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AttendanceRecord>()
            .HasIndex(ar => new { ar.EmployeeId, ar.Date })
            .IsUnique();
    }

    private void ConfigureDocument(ModelBuilder builder)
    {
        builder.Entity<Document>()
            .HasOne(d => d.Employee)
            .WithMany(e => e.Documents)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureRole(ModelBuilder builder)
    {
        builder.Entity<Role>()
            .ToTable("HRMRoles")
            .HasIndex(r => r.Name)
            .IsUnique();
    }

    private void ConfigureRoleAssignment(ModelBuilder builder)
    {
        builder.Entity<RoleAssignment>()
            .HasOne(ra => ra.Role)
            .WithMany(r => r.RoleAssignments)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RoleAssignment>()
            .HasOne(ra => ra.Branch)
            .WithMany(b => b.RoleAssignments)
            .HasForeignKey(ra => ra.BranchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<RoleAssignment>()
            .HasIndex(ra => new { ra.UserId, ra.RoleId, ra.BranchId });
    }

    private void ConfigureApprovalRule(ModelBuilder builder)
    {
        builder.Entity<ApprovalRule>()
            .HasOne(ar => ar.Branch)
            .WithMany(b => b.ApprovalRules)
            .HasForeignKey(ar => ar.BranchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApprovalRule>()
            .HasOne(ar => ar.Department)
            .WithMany(d => d.ApprovalRules)
            .HasForeignKey(ar => ar.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApprovalRule>()
            .HasIndex(ar => new { ar.EntityType, ar.BranchId, ar.DepartmentId });
    }

    private void ConfigureApprovalStep(ModelBuilder builder)
    {
        builder.Entity<ApprovalStep>()
            .HasOne(step => step.ApprovalRule)
            .WithMany(ar => ar.Steps)
            .HasForeignKey(step => step.ApprovalRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApprovalStep>()
            .HasOne(step => step.Role)
            .WithMany(r => r.ApprovalSteps)
            .HasForeignKey(step => step.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApprovalStep>()
            .HasIndex(step => new { step.ApprovalRuleId, step.StepOrder });
    }

    private void ConfigureAuditLog(ModelBuilder builder)
    {
        builder.Entity<AuditLog>()
            .HasIndex(al => new { al.EntityType, al.EntityId });
        
        builder.Entity<AuditLog>()
            .HasIndex(al => al.Timestamp);
        
        builder.Entity<AuditLog>()
            .HasIndex(al => al.ActorId);
    }

    private void ConfigureSetting(ModelBuilder builder)
    {
        builder.Entity<Setting>()
            .HasIndex(s => new { s.Key, s.Environment, s.Version });
    }

    private void ConfigureNotification(ModelBuilder builder)
    {
        builder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead });
        
        builder.Entity<Notification>()
            .HasIndex(n => n.CreatedAt);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update UpdatedAt timestamp
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        // Audit logging will be handled by a separate interceptor/service
        // to avoid circular dependencies and performance issues
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}

