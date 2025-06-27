using System.Diagnostics.CodeAnalysis;
using ProjectTemplate.Application.Common.Security;
using ProjectTemplate.Domain.Enums;


namespace ProjectTemplate.Web.Infrastructure;

public static class EndpointRouteBuilderExtensions
{

    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, params EnumPermission[] permissions)
    => MapGet(builder, handler, handler.Method.Name, permissions);

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, params EnumPermission[] permissions)
    => MapPost(builder, handler, handler.Method.Name, permissions);

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, params EnumPermission[] permissions)
    => MapPut(builder, handler, handler.Method.Name, permissions);

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, params EnumPermission[] permissions)
    => MapDelete(builder, handler, handler.Method.Name, permissions);

    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "", params EnumPermission[] permissions)
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapGet(pattern, handler)
              .WithName(handler.Method.Name)
              .RequiredPermission(permissions);

        return builder;
    }

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "", params EnumPermission[] permissions)
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPost(pattern, handler)
            .WithName(handler.Method.Name)
            .RequiredPermission(permissions);

        return builder;
    }

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern, params EnumPermission[] permissions)
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPut(pattern, handler)
            .WithName(handler.Method.Name)
            .RequiredPermission(permissions);

        return builder;
    }

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern, params EnumPermission[] permissions)
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapDelete(pattern, handler)
            .WithName(handler.Method.Name)
            .RequiredPermission(permissions);

        return builder;
    }
}

