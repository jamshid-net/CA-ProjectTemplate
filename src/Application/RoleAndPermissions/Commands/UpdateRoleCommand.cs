using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Entities.Auth;

namespace ProjectTemplate.Application.RoleAndPermissions.Commands;
public record UpdateRoleCommand(int Id, string Name, int[] PermissionIds) : IRequest<bool>;
public class UpdateRoleCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateRoleCommand, bool>
{
    public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);
        if (role == null)
        {
            throw new NotFoundException(request.Id.ToString(), nameof(Role));
        }
        role.Name = request.Name;
        role.Permissions = await dbContext.Permissions
                                          .Where(p => request.PermissionIds.Contains(p.Id))
                                          .ToListAsync(cancellationToken);

        dbContext.Roles.Update(role);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;

    }
}
