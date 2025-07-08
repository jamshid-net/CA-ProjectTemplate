using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Enums;
using Serilog;

namespace ProjectTemplate.Infrastructure.Services;
public class PermissionCacheService(IDistributedCache cache) : IPermissionCacheService
{
    private const string RoleKeyPrefix = "ROLE_ID";

    public async Task<List<EnumPermission>> GetPermissionsAsync(int roleId, CancellationToken ct = default)
    {
        if (roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "Role ID must be greater than zero.");
       
        var values = await cache.GetStringAsync($"{RoleKeyPrefix}:{roleId}", ct);

        if(string.IsNullOrEmpty(values))
            return [];

        return JsonSerializer.Deserialize<List<EnumPermission>>(values) ?? [];

    }

    public async Task<bool> SetPermissionAsync(int roleId, EnumPermission[] permissions, CancellationToken ct = default)
    {
        if(roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "Role ID must be greater than zero.");

        try
        {
            if (permissions is null or [])
                await cache.SetStringAsync($"{RoleKeyPrefix}:{roleId}", string.Empty, ct);

            string json = JsonSerializer.Serialize(permissions);
            await cache.SetStringAsync($"{RoleKeyPrefix}:{roleId}", json, ct);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error setting permissions for role ID {RoleId}", roleId);
            return false;
        }
    }

    public async Task<bool> RemoveRoleAsync(int roleId, CancellationToken ct = default)
    {
        try
        {
           await cache.RemoveAsync($"{RoleKeyPrefix}:{roleId}", ct);
           return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error removing role ID {RoleId} from cache", roleId);
            return false;
        }
    }
}
