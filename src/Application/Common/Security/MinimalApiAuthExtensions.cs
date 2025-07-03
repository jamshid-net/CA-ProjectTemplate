using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            if (user?.Identity is not { IsAuthenticated: true })
                return Results.Unauthorized();

            // 2. User ID
            //if (!int.TryParse(user.FindFirst(StaticClaims.UserId)?.Value, out int userId))
            //    return Results.Unauthorized();

            var cacheService = httpContext.RequestServices.GetRequiredService<IPostgresCacheService>();
            //3.Blacklist check(resolve SystemCacheService)
            //if (await systemCache.IsUserBlackListed(userId))
            //    return Results.Unauthorized();

            // 4. Permissions check
            if (enumPermissions is { Length: > 0 })
            {
                if (!int.TryParse(user?.FindFirst(StaticClaims.RoleId)?.Value, out int userRoleId))
                    return Results.Forbid();

                var permissions = await cacheService.GetAsync<List<EnumPermission>>($"role_id:{userRoleId}");
                if (permissions is not null && !permissions.Intersect(enumPermissions).Any())
                {
                    // Custom forbidden response
                    var msg = $"Missing required permissions: {string.Join(", ", enumPermissions)}";
                    //return Results.Problem(detail: msg, statusCode: StatusCodes.Status403Forbidden);
                    return Results.Forbid();
                }
            }

            return await next(context);
        });

        return builder;
    }
}

