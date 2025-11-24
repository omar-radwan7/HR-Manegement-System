namespace HRM.Application.Services;

public interface IDocumentService
{
    Task<DocumentDto> UploadDocumentAsync(Guid employeeId, Stream fileStream, string fileName, string contentType);
    Task<DocumentDto?> GetDocumentAsync(Guid documentId);
    Task<bool> DeleteDocumentAsync(Guid documentId);
    Task<bool> TagPiiAsync(Guid documentId, List<string> tags);
}

public class DocumentDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public List<string> PiiTags { get; set; } = new();
}

