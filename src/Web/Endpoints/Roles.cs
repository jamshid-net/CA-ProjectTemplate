using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Application.Common.QueryFilter;
using ProjectTemplate.Application.RoleAndPermissions.Commands;
using ProjectTemplate.Application.RoleAndPermissions.Queries;

namespace ProjectTemplate.Web.Endpoints;

public class Roles : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(CreateRole);
        group.MapPut(UpdateRole);
        group.MapDelete(DeleteRole);
        group.MapGet(GetRoleDetails);          
        group.MapPost(GetRoles);
    }

    public async Task<Results<Created, BadRequest>> CreateRole(
        ISender sender,
        [FromBody] CreateRoleCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result ? TypedResults.Created() : TypedResults.BadRequest();
    }

    public async Task<Results<Ok, BadRequest>> UpdateRole(
        ISender sender,
        [FromBody] UpdateRoleCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result ? TypedResults.Ok() : TypedResults.BadRequest();
    }

    public async Task<Results<Ok, BadRequest>> DeleteRole(
        ISender sender,
        [FromBody] DeleteRoleCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result ? TypedResults.Ok() : TypedResults.BadRequest();
    }


    public async Task<Ok<RoleDetailsDto>> GetRoleDetails(ISender sender, int roleId, CancellationToken ct)
    {
        var role = await sender.Send(new GetRoleDetailQuery(roleId), ct);
        return TypedResults.Ok(role);
    }

    public async Task<Ok<PageList<RoleDto>>> GetRoles(ISender sender, [FromBody] FilterRequest filterRequest, CancellationToken ct)
    {
        var roles = await sender.Send(new GetRolesQuery(filterRequest), ct);
        return TypedResults.Ok(roles);
    }
}
