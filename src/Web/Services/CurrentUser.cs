using System.Security.Claims;

using ProjectTemplate.Application.Common.Interfaces;

namespace ProjectTemplate.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
