using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRM.Application.Services;

namespace HRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<string>> GetSetting(string key, [FromQuery] string? environment)
    {
        var value = await _settingsService.GetSettingAsync(key, environment);
        if (value == null)
            return NotFound();

        return Ok(value);
    }

    [HttpGet]
    public async Task<ActionResult<List<SettingDto>>> GetAllSettings([FromQuery] string? environment)
    {
        var settings = await _settingsService.GetAllSettingsAsync(environment);
        return Ok(settings);
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingRequest request)
    {
        await _settingsService.UpdateSettingAsync(key, request.Value, request.EffectiveDate, request.Environment);
        return Ok();
    }
}

public class UpdateSettingRequest
{
    public string Value { get; set; } = string.Empty;
    public DateTime? EffectiveDate { get; set; }
    public string? Environment { get; set; }
}

