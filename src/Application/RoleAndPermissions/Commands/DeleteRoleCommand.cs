using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Shared.PostgresqlCache;
using Serilog;

namespace ProjectTemplate.Application.RoleAndPermissions.Commands;
public record DeleteRoleCommand(int RoleId) : IRequest<bool>;

public class DeleteRoleCommandHandler(IApplicationDbContext dbContext, IPostgresCacheService cacheService) : IRequestHandler<DeleteRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var effectedRows = await dbContext.Roles.Where(x => x.Id == request.RoleId).ExecuteDeleteAsync(cancellationToken);
        
        if (effectedRows > 0)
        {
            try
            {
                await cacheService.RemoveAsync($"role_id:{request.RoleId}", cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error removing role permissions from cache for role ID {RoleId}", request.RoleId);
            }
        }
        return effectedRows > 0;
    }
}
