using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Application.Common.QueryFilter;
using ProjectTemplate.Application.Users.Manage.Commands;
using ProjectTemplate.Application.Users.Manage.Queries;
using ProjectTemplate.Domain.Enums;

namespace ProjectTemplate.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetUsers, EnumPermission.CreateUser)
            .MapPost(CreateUser, EnumPermission.CreateUser);

    }

    public async Task<Ok<List<UserDto>>> GetUsers(ISender sender, [Required] FilterRequest filter)
    {
        var res = await sender.Send(new GetUsers(filter));

        return TypedResults.Ok(res);
    }

    public async Task<Results<Ok, BadRequest>> CreateUser(ISender sender, [Required] CreateUserCommand createUserCommand)
    {
        var res = await sender.Send(createUserCommand);

        return res ? TypedResults.Ok() : TypedResults.BadRequest();

    }
}
