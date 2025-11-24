using HRM.Domain.Entities;

namespace HRM.Application.Services;

public interface IApprovalEngine
{
    Task<List<ApprovalStep>> ResolveApproversAsync(string entityType, Guid? entityId, Guid? branchId, Guid? departmentId);
    Task<bool> ProcessApprovalAsync(Guid workflowId, string approverId, bool approved, string? comments);
    Task<List<object>> GetPendingApprovalsAsync(string userId);
}

