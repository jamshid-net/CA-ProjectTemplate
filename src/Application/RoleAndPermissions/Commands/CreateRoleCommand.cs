using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Entities.Auth;
using ProjectTemplate.Domain.Enums;
using ProjectTemplate.Shared.PostgresqlCache;
using Serilog;

namespace ProjectTemplate.Application.RoleAndPermissions.Commands;
public record CreateRoleCommand(string Name, int[] PermissionIds) : IRequest<bool>;
public class CreateRoleCommandHandler(IApplicationDbContext dbContext, IPostgresCacheService cacheService) : IRequestHandler<CreateRoleCommand, bool>
{
    public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Role name cannot be null or empty.", nameof(request.Name));
        }
        var foundPermissions = await dbContext.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (foundPermissions.Count != request.PermissionIds.Length)
        {
            throw new InvalidOperationException("One or more permission IDs are invalid.");
        }



        var role = new Role
        {
            Name = request.Name,
            Permissions = foundPermissions
        };
       
        dbContext.Roles.Add(role);
        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0 ;

        if (isSaved)
        {
            try
            {
                await cacheService.SetAsync(new CacheItem<EnumPermission[]>(
                    $"role_id:{role.Id}",
                    role.Permissions.Select(p => p.EnumPermission).ToArray(),
                    null), cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error updating role permissions in cache for role ID {RoleId}", role.Id);
            }
            
        }
        return isSaved;
        
    }
}
