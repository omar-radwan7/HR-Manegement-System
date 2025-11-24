using Microsoft.EntityFrameworkCore;
using HRM.Application.Services;
using HRM.Domain.Entities;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ApplicationDbContext _context;

    public SettingsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetSettingAsync(string key, string? environment = null)
    {
        var now = DateTime.UtcNow;
        var query = _context.Settings
            .Where(s => s.Key == key && s.EffectiveDate <= now);

        if (!string.IsNullOrEmpty(environment))
        {
            query = query.Where(s => s.Environment == null || s.Environment == environment);
        }
        else
        {
            query = query.Where(s => s.Environment == null);
        }

        query = query.OrderByDescending(s => s.Version).ThenByDescending(s => s.EffectiveDate);

        var setting = await query.FirstOrDefaultAsync();
        return setting?.Value;
    }

    public async Task<bool> UpdateSettingAsync(string key, string value, DateTime? effectiveDate = null, string? environment = null)
    {
        var existing = await _context.Settings
            .Where(s => s.Key == key && (s.Environment == environment || (s.Environment == null && environment == null)))
            .OrderByDescending(s => s.Version)
            .FirstOrDefaultAsync();

        var newVersion = existing?.Version + 1 ?? 1;

        var setting = new Setting
        {
            Key = key,
            Value = value,
            Version = newVersion,
            EffectiveDate = effectiveDate ?? DateTime.UtcNow,
            Environment = environment
        };

        _context.Settings.Add(setting);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<SettingDto>> GetAllSettingsAsync(string? environment = null)
    {
        var query = _context.Settings.AsQueryable();

        if (!string.IsNullOrEmpty(environment))
        {
            query = query.Where(s => s.Environment == null || s.Environment == environment);
        }
        else
        {
            query = query.Where(s => s.Environment == null);
        }

        var settings = await query.OrderBy(s => s.Key).ThenByDescending(s => s.Version).ToListAsync();

        return settings.Select(s => new SettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Version = s.Version,
            EffectiveDate = s.EffectiveDate,
            Environment = s.Environment,
            Description = s.Description
        }).ToList();
    }
}

