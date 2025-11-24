namespace HRM.Domain.Entities;

public class Position : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Department Department { get; set; } = null!;
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

