using Microsoft.EntityFrameworkCore;
using HRM.Domain.Entities;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public class ApprovalEngine : IApprovalEngine
{
    private readonly ApplicationDbContext _context;

    public ApprovalEngine(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApprovalStep>> ResolveApproversAsync(string entityType, Guid? entityId, Guid? branchId, Guid? departmentId)
    {
        // Query approval rules matching the criteria
        // Priority: branch-specific > department-specific > global
        var rules = await _context.ApprovalRules
            .Where(ar => ar.EntityType == entityType && ar.IsActive)
            .Where(ar => 
                (ar.BranchId == branchId && ar.DepartmentId == departmentId) || // Branch + Department
                (ar.BranchId == branchId && ar.DepartmentId == null) || // Branch only
                (ar.BranchId == null && ar.DepartmentId == departmentId) || // Department only
                (ar.BranchId == null && ar.DepartmentId == null)) // Global
            .OrderByDescending(ar => ar.Priority) // Higher priority first
            .ThenByDescending(ar => ar.BranchId.HasValue ? 1 : 0) // Branch-specific first
            .ThenByDescending(ar => ar.DepartmentId.HasValue ? 1 : 0) // Department-specific first
            .ToListAsync();

        if (!rules.Any())
            return new List<ApprovalStep>();

        // Get the most specific rule (first in the ordered list)
        var rule = rules.First();

        // Get all steps for this rule, ordered by StepOrder
        var steps = await _context.ApprovalSteps
            .Where(step => step.ApprovalRuleId == rule.Id)
            .OrderBy(step => step.StepOrder)
            .ToListAsync();

        return steps;
    }

    public async Task<bool> ProcessApprovalAsync(Guid workflowId, string approverId, bool approved, string? comments)
    {
        // TODO: Implement workflow processing
        // This would involve:
        // 1. Finding the workflow instance
        // 2. Updating approval status
        // 3. Moving to next step if sequential
        // 4. Checking if all parallel steps are complete
        // 5. Finalizing approval if complete
        
        await Task.CompletedTask;
        return true;
    }

    public async Task<List<object>> GetPendingApprovalsAsync(string userId)
    {
        // TODO: Implement pending approvals retrieval
        // This would query workflows where:
        // - The user is an approver
        // - The step is pending
        // - The workflow is not completed
        
        await Task.CompletedTask;
        return new List<object>();
    }
}

