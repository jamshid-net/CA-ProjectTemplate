using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.Common.QueryFilter;

namespace ProjectTemplate.Application.Users.Manage.Queries;

public record GetUsers(FilterRequest PageRequest) : IRequest<List<UserDto>>;

public class GetUsersHandler(IApplicationDbContext dbContext) : IRequestHandler<GetUsers, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetUsers request, CancellationToken cancellationToken)
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
          .ApplyPageRequest(request.PageRequest)
          .ToListAsync(cancellationToken);
    }
}

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
