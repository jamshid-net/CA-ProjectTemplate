using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Domain.Enums;
using ProjectTemplate.Shared.Constants;
using ProjectTemplate.Shared.PostgresqlCache;

namespace ProjectTemplate.Application.Common.Security;
public static class MinimalApiAuthExtensions
{
    public static RouteHandlerBuilder RequiredPermission(
        this RouteHandlerBuilder builder,
        params EnumPermission[] enumPermissions)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;

            // 1. Authenticated?
            if (user?.Identity is { IsAuthenticated: false })
                return Results.Unauthorized();

            // 2. Permissions check
            if (enumPermissions is { Length: > 0 })
            {
                var cacheService = httpContext.RequestServices.GetRequiredService<IPostgresCacheService>();

                if (!int.TryParse(user?.FindFirst(StaticClaims.RoleId)?.Value, out int userRoleId))
                    return Results.Forbid();

                var permissions = await cacheService.GetAsync<List<EnumPermission>>($"role_id:{userRoleId}");
                if (permissions is not null && !permissions.Intersect(enumPermissions).Any())
                {
                    // Custom forbidden response
                    //var msg = $"Missing required permissions: {string.Join(", ", enumPermissions)}";
                    //return Results.Problem(detail: msg, statusCode: StatusCodes.Status403Forbidden);
                    return Results.Forbid();
                }
            }

            return await next(context);
        });

        return builder;
    }
}

