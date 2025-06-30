using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProjectTemplate.Domain.Enums;

namespace ProjectTemplate.Application.Common.Security;
public static class MinimalApiAuthExtensions
{
    public static RouteHandlerBuilder RequiredPermission(
        this RouteHandlerBuilder builder,
        EnumPermission[] enumPermissions)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            //var httpContext = context.HttpContext;
            //var user = httpContext.User;

            //// 1. Authenticated?
            //if (user?.Identity is not { IsAuthenticated: true })
            //    return Results.Unauthorized();

            //// 2. User ID
            //if (!int.TryParse(user.FindFirst("UserId")?.Value, out int userId))
            //    return Results.Unauthorized();

            //// 3. Blacklist check (resolve SystemCacheService)
            ////var systemCache = httpContext.RequestServices.GetRequiredService<SystemCacheService>();
            ////if (await systemCache.IsUserBlackListed(userId))
            ////    return Results.Unauthorized();

            //// 4. Permissions check
            //if (enumPermissions?.Length > 0)
            //{
            //    if (!int.TryParse(user.FindFirst("RoleId")?.Value, out int userRoleId))
            //        return Results.Forbid();

            //    //var permissions = await systemCache.GetPermissions(userRoleId);
            //    //if (!permissions.Intersect(enumPermissions).Any())
            //    //{
            //    //    // Custom forbidden response
            //    //    var msg = $"Missing required permissions: {string.Join(", ", enumPermissions)}";
                    
            //    //}
            //    return Results.Forbid();
            //}

            //// All good, proceed
            return await next(context);
        });

        return builder;
    }
}
