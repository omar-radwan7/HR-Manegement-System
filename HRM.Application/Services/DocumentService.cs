using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using HRM.Domain.Entities;
using HRM.Infrastructure.Data;
using System.Text.Json;

namespace HRM.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DocumentService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<DocumentDto> UploadDocumentAsync(Guid employeeId, Stream fileStream, string fileName, string contentType)
    {
        // TODO: Implement virus scanning hook
        // TODO: Implement blob storage or signed URLs
        
        var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads", employeeId.ToString());
        Directory.CreateDirectory(uploadsPath);

        var fileId = Guid.NewGuid();
        var filePath = Path.Combine(uploadsPath, $"{fileId}_{fileName}");

        using (var file = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(file);
        }

        var document = new Document
        {
            EmployeeId = employeeId,
            Name = fileName,
            FilePath = filePath,
            FileSize = new FileInfo(filePath).Length,
            ContentType = contentType,
            VirusScanned = false, // TODO: Implement virus scanning
            PiiTags = JsonSerializer.Serialize(new List<string>())
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return new DocumentDto
        {
            Id = document.Id,
            EmployeeId = document.EmployeeId,
            Name = document.Name,
            FileSize = document.FileSize,
            ContentType = document.ContentType,
            UploadedAt = document.UploadedAt,
            PiiTags = JsonSerializer.Deserialize<List<string>>(document.PiiTags ?? "[]") ?? new List<string>()
        };
    }

    public async Task<DocumentDto?> GetDocumentAsync(Guid documentId)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null)
            return null;

        return new DocumentDto
        {
            Id = document.Id,
            EmployeeId = document.EmployeeId,
            Name = document.Name,
            FileSize = document.FileSize,
            ContentType = document.ContentType,
            UploadedAt = document.UploadedAt,
            PiiTags = JsonSerializer.Deserialize<List<string>>(document.PiiTags ?? "[]") ?? new List<string>()
        };
    }

    public async Task<bool> DeleteDocumentAsync(Guid documentId)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null)
            return false;

        // Soft delete - set retention date
        document.RetentionDate = DateTime.UtcNow.AddDays(30);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TagPiiAsync(Guid documentId, List<string> tags)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null)
            return false;

        document.PiiTags = JsonSerializer.Serialize(tags);
        await _context.SaveChangesAsync();

        return true;
    }
}

