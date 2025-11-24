namespace HRM.Domain.Entities;

public class Document : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RetentionDate { get; set; }
    public string? PiiTags { get; set; } // JSON array of PII tags
    public bool VirusScanned { get; set; } = false;

    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
}

