namespace HRM.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? ContractId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Department Department { get; set; } = null!;
    public virtual Position Position { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual Contract? Contract { get; set; } // Current active contract
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}

