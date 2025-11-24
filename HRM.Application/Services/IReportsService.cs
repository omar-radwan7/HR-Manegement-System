namespace HRM.Application.Services;

public interface IReportsService
{
    Task<HeadcountReportDto> GetHeadcountReportAsync(Guid? branchId, DateTime? date);
    Task<AttritionReportDto> GetAttritionReportAsync(Guid? branchId, DateTime startDate, DateTime endDate);
    Task<List<LeaveBalanceReportDto>> GetLeaveBalancesReportAsync(Guid? branchId);
    Task<AbsenceReportDto> GetAbsenceReportAsync(Guid? branchId, DateTime startDate, DateTime endDate);
}

public class HeadcountReportDto
{
    public DateTime ReportDate { get; set; }
    public List<DepartmentHeadcount> ByDepartment { get; set; } = new();
    public List<PositionHeadcount> ByPosition { get; set; } = new();
}

public class DepartmentHeadcount
{
    public string DepartmentName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class PositionHeadcount
{
    public string PositionTitle { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AttritionReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalAttrition { get; set; }
    public List<AttritionByDepartment> ByDepartment { get; set; } = new();
}

public class AttritionByDepartment
{
    public string DepartmentName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class LeaveBalanceReportDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveTypeName { get; set; } = string.Empty;
    public int Balance { get; set; }
}

public class AbsenceReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<AbsenceByEmployee> ByEmployee { get; set; } = new();
}

public class AbsenceByEmployee
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int AbsenceDays { get; set; }
}

