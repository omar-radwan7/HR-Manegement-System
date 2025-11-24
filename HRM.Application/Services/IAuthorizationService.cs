using HRM.Domain.Entities;
using HRM.Infrastructure.Data;

namespace HRM.Application.Services;

public interface IAuthorizationService
{
    Task<bool> CanReadAsync<T>(T entity, ApplicationUser user, Guid? branchId) where T : class;
    Task<bool> CanWriteAsync<T>(T entity, ApplicationUser user, Guid? branchId) where T : class;
    Task<bool> CanApproveAsync<T>(T entity, ApplicationUser user, Guid? branchId) where T : class;
    bool ShouldMaskField(string fieldName, ApplicationUser user);
}

