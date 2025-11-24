using Xunit;
using HRM.Application.Services;
using HRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HRM.Tests.Unit.Services;

public class ApprovalEngineTests
{
    [Fact]
    public async Task ResolveApproversAsync_ReturnsEmptyList_WhenNoRulesExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var engine = new ApprovalEngine(context);

        // Act
        var result = await engine.ResolveApproversAsync("LeaveRequest", null, null, null);

        // Assert
        Assert.Empty(result);
    }

    // Add more tests as needed
}

