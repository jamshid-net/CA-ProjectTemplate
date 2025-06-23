using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Entities;
using ProjectTemplate.Shared.Helpers;

namespace ProjectTemplate.Application.Users.Commands;
public class CreateUserCommand : IRequest<bool>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
public class CreateUserCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateUserCommand, bool>
{

    public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var hashSalt = CryptoPassword.CreateHashSalted(request.Password);
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic,
            UserName = request.UserName,
            Email = request.Email,
            IsActive = request.IsActive,
            PasswordHash = hashSalt.Hash,
            PasswordSalt = hashSalt.Salt,
            LastLogin = DateTimeOffset.UtcNow,
            
        };
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
