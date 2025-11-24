using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HRM.Infrastructure.Data;

namespace HRM.Infrastructure.BackgroundJobs;

public class DocumentRetentionJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DocumentRetentionJob> _logger;

    public DocumentRetentionJob(ApplicationDbContext context, ILogger<DocumentRetentionJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessRetention()
    {
        _logger.LogInformation("Starting document retention job at {Time}", DateTime.UtcNow);

        var expiredDocuments = await _context.Documents
            .Where(d => d.RetentionDate.HasValue && d.RetentionDate.Value <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var document in expiredDocuments)
        {
            try
            {
                // Delete physical file
                if (System.IO.File.Exists(document.FilePath))
                {
                    System.IO.File.Delete(document.FilePath);
                }

                // Delete database record
                _context.Documents.Remove(document);

                // TODO: Create audit log entry
                
                _logger.LogInformation("Deleted document {DocumentId} - {FileName}", document.Id, document.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId}", document.Id);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Completed document retention job. Deleted {Count} documents", expiredDocuments.Count);
    }
}

