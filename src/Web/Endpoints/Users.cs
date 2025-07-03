using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Application.Common.Models;
using ProjectTemplate.Application.Common.QueryFilter;
using ProjectTemplate.Application.Common.Security;
using ProjectTemplate.Application.Users.Auth.Commands;
using ProjectTemplate.Application.Users.Manage.Commands;
using ProjectTemplate.Application.Users.Manage.Queries;
using ProjectTemplate.Domain.Enums;

namespace ProjectTemplate.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(GetUsers);
        group.MapPost(CreateUser).RequiredPermission(EnumPermission.CreateUser);
        group.MapPost(Login);
    }

    public async Task<Ok<PageList<UserDto>>> GetUsers(ISender sender, [FromBody] GetUserFilterQuery filter)
    {
        var res = await sender.Send(filter);

        return TypedResults.Ok(res);
    }

    public async Task<Results<Ok, BadRequest>> CreateUser(ISender sender, [FromBody] CreateUserCommand createUserCommand)
    {
        var res = await sender.Send(createUserCommand);

        return res ? TypedResults.Ok() : TypedResults.BadRequest();

    }

    public async Task<Ok<TokenResponseModel>> Login(ISender sender, [FromBody] LoginCommand command)
    {
        var res = await sender.Send(command);
        return TypedResults.Ok(res);
    }
}
