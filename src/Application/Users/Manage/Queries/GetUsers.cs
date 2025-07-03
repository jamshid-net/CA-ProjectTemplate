using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.Common.QueryFilter;

namespace ProjectTemplate.Application.Users.Manage.Queries;

public record GetUsers(FilterRequest PageRequest) : IRequest<PageList<UserDto>>;

public class GetUsersHandler(IApplicationDbContext dbContext, IMapper mapper) : IRequestHandler<GetUsers, PageList<UserDto>>
{
    public async Task<PageList<UserDto>> Handle(GetUsers request, CancellationToken cancellationToken)
    {
        return await dbContext.Users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Patronymic = u.Patronymic,
            UserName = u.UserName,
            IsActive = u.IsActive
        }).AsNoTracking()
          .ProjectTo<UserDto>(mapper.ConfigurationProvider)
          .ToPageListAsync(request.PageRequest, cancellationToken);
          
    }
}

