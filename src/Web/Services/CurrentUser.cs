using System.Security.Claims;

using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Shared.Constants;

namespace ProjectTemplate.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(StaticClaims.UserId);
    public string? RoleId => httpContextAccessor.HttpContext?.User?.FindFirstValue(StaticClaims.RoleId);
}
