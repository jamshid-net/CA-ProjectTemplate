using ProjectTemplate.Domain.Enums;

namespace ProjectTemplate.Application.Common.Interfaces;
public interface IPermissionCacheService
{
    public Task<List<EnumPermission>> GetPermissionsAsync(int roleId, CancellationToken ct = default);
    public Task<bool> SetPermissionAsync(int roleId, EnumPermission[] permissions, CancellationToken ct = default);
    public Task<bool> RemoveRoleAsync(int roleId, CancellationToken ct = default);
}
