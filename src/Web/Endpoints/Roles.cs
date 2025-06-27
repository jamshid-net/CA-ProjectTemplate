using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Application.Common.Models;
using ProjectTemplate.Application.RoleAndPermissions.Commands;

namespace ProjectTemplate.Web.Endpoints;

public class Roles : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .MapPost(CreateRole);
    }

    public async Task<Results<Created, BadRequest>> CreateRole(
        ISender sender,
        [FromBody] CreateRoleCommand command,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result ? TypedResults.Created() : TypedResults.BadRequest();
    }
}
