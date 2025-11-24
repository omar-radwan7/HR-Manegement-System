using Microsoft.AspNetCore.Http;
using HRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRM.Api.Middleware;

public class BranchScopeMiddleware
{
    private readonly RequestDelegate _next;
    private const string BranchIdHeader = "X-Branch-Id";
    private const string BranchIdItemKey = "BranchId";

    public BranchScopeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        // Skip branch validation for public endpoints
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/api/auth") || 
            path.Contains("/swagger") || 
            path.Contains("/hangfire") ||
            path.Contains("/health"))
        {
            await _next(context);
            return;
        }

        // Check if user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        // Read branch ID from header
        if (!context.Request.Headers.TryGetValue(BranchIdHeader, out var branchIdHeaderValue))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("X-Branch-Id header is required");
            return;
        }

        if (!Guid.TryParse(branchIdHeaderValue.ToString(), out var branchId))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid X-Branch-Id format");
            return;
        }

        // Validate branch exists
        var branchExists = await dbContext.Branches.AnyAsync(b => b.Id == branchId && b.IsActive);
        if (!branchExists)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Branch not found or inactive");
            return;
        }

        // TODO: Validate user has access to this branch
        // Check if user has global access or branch-specific access
        var userId = context.User.Identity.Name;
        // var hasAccess = await ValidateUserBranchAccess(userId, branchId, dbContext);
        // if (!hasAccess)
        // {
        //     context.Response.StatusCode = 403;
        //     await context.Response.WriteAsync("User does not have access to this branch");
        //     return;
        // }

        // Store branch ID in context for downstream use
        context.Items[BranchIdItemKey] = branchId;

        await _next(context);
    }

    public static Guid? GetBranchId(HttpContext context)
    {
        if (context.Items.TryGetValue(BranchIdItemKey, out var branchId) && branchId is Guid id)
        {
            return id;
        }
        return null;
    }
}

