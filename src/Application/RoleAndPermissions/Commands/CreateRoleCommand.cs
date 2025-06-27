using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Entities.Auth;

namespace ProjectTemplate.Application.RoleAndPermissions.Commands;
public record CreateRoleCommand(string Name, int[] PermissionIds) : IRequest<bool>;
public class CreateRoleCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateRoleCommand, bool>
{
    public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Role name cannot be null or empty.", nameof(request.Name));
        }
        var role = new Role
        {
            Name = request.Name,
            Permissions = await dbContext.Permissions
                                         .Where(p => request.PermissionIds.Contains(p.Id))
                                         .ToListAsync(cancellationToken)
        };
        await dbContext.Roles.AddAsync(role, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0 ;
        
    }
}
