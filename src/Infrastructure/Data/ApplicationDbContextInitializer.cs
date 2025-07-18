﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Entities.Auth;
using ProjectTemplate.Domain.Enums;

namespace ProjectTemplate.Infrastructure.Data;

public static class InitializerExtensions
{
    public static void AddAsyncSeeding(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        builder.UseAsyncSeeding(async (context, _, ct) =>
        {
            var initializer = serviceProvider.GetRequiredService<ApplicationDbContextInitializer>();

            await initializer.SeedAsync();
        });
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitialiseAsync();
    }
}

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    IPermissionCacheService cacheService,
    ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedPermissionsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
        finally
        {
            await LoadPermissionToCacheAsync();
        }
    }

    public async Task TrySeedPermissionsAsync()
    {
        var defaultPermissions = DefaultPermissions();
        var existingPermissions = await context.Permissions
            .IgnoreQueryFilters()
            .Where(p => defaultPermissions.Select(dp => dp.Id).Contains(p.Id))
            .ToListAsync();

        foreach (var permission in defaultPermissions)
        {
            var existingPermission = existingPermissions.FirstOrDefault(p => p.Id == permission.Id);

            if (existingPermission == null)
            {
                // Insert the new permission without the need for IDENTITY_INSERT
                await context.Permissions.AddAsync(permission);
                await context.SaveChangesAsync();
            }
            else
            {
                // Update existing permission if necessary
                if (existingPermission.Name != permission.Name || existingPermission.EnumPermission != permission.EnumPermission)
                {
                    existingPermission.EnumPermission = permission.EnumPermission;
                    context.Permissions.Update(existingPermission);
                    await context.SaveChangesAsync();
                }
            }
        }

        var defaultPermissionIds = defaultPermissions.Select(dp => dp.Id);
        await context.Permissions.Where(p => !defaultPermissionIds.Contains(p.Id))
            .ExecuteDeleteAsync();
    }

    private async Task LoadPermissionToCacheAsync()
    {
        var roleIdAndPermissions = await context.Roles.AsNoTracking()
                     .Select(x => new
                     {
                         RoleId = x.Id,
                         EnumPermissions = x.Permissions.Select(p => p.EnumPermission)
                     }).ToDictionaryAsync(x => x.RoleId, y => y.EnumPermissions.ToArray());


        foreach (var roleIdAndPermission in roleIdAndPermissions)
        {
            await cacheService.SetPermissionAsync(roleIdAndPermission.Key, roleIdAndPermission.Value);
        }
    }

    private static Permission[] DefaultPermissions()
    {
        var enumPermissions = Enum.GetValues<EnumPermission>();

        var authPermissions = enumPermissions.Select(permission =>
        {
            if (permission == 0)
            {
                throw new InvalidOperationException($"{permission.ToString()} cannot be start value from 0. Permission ID cannot be 0.");
            }
            return new Permission
            {
                Id = (int)permission,
                EnumPermission = permission
            };
        }).ToArray();
        return authPermissions;
    }
}
