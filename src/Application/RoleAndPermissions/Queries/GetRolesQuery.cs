using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.Common.QueryFilter;

namespace ProjectTemplate.Application.RoleAndPermissions.Queries;
public record GetRolesQuery(FilterRequest FilterRequest) : IRequest<PageList<RoleDto>>;
public class GetRolesQueryHandler(IApplicationDbContext dbContext, IMapper mapper) : IRequestHandler<GetRolesQuery, PageList<RoleDto>>
{
    public async Task<PageList<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var result = await dbContext.Roles
                                              .AsNoTracking()
                                              .ProjectTo<RoleDto>(mapper.ConfigurationProvider)
                                              .ToPageListAsync(request.FilterRequest, cancellationToken);

        return result;
    }
}

