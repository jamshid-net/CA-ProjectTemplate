using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Entities.Auth;
using ProjectTemplate.Domain.Enums;
using ProjectTemplate.Shared.PostgresqlCache;
using Serilog;

namespace ProjectTemplate.Application.RoleAndPermissions.Commands;
public record UpdateRoleCommand(int Id, string Name, int[] PermissionIds) : IRequest<bool>;
public class UpdateRoleCommandHandler(IApplicationDbContext dbContext, IPostgresCacheService cacheService) : IRequestHandler<UpdateRoleCommand, bool>
{
    public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (role == null)
        {
            throw new NotFoundException(request.Id.ToString(), nameof(Role));
        }
        role.Name = request.Name;
        role.Permissions = await dbContext.Permissions
                                          .Where(p => request.PermissionIds.Contains(p.Id))
                                          .ToListAsync(cancellationToken);

        dbContext.Roles.Update(role);
        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0;

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
