using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.Common.QueryFilter;

namespace ProjectTemplate.Application.Users.Manage.Queries;

public class GetUserFilterQuery : FilterRequest, IRequest<PageList<UserDto>>;

public class GetUserFilterQueryHandler(IApplicationDbContext dbContext, ICustomIdentityService customIdentity, ICurrentUser currentUser,IMapper mapper) : IRequestHandler<GetUserFilterQuery, PageList<UserDto>>
{
    public async Task<PageList<UserDto>> Handle(GetUserFilterQuery request, CancellationToken cancellationToken)
    {
        var userName = await customIdentity.GetUserNameAsync(currentUser.Id, cancellationToken);

        return await dbContext.Users
                              .AsNoTracking()
                              .ProjectTo<UserDto>(mapper.ConfigurationProvider)
                              .ToPageListAsync(request, cancellationToken);

    }
}

