using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Application.TodoLists.Commands.DeleteTodoList;
using ProjectTemplate.Application.TodoLists.Commands.UpdateTodoList;
using ProjectTemplate.Application.TodoLists.Queries.GetTodos;

namespace ProjectTemplate.Web.Endpoints;

public class TodoLists : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet(GetTodoLists);
        group.MapPost(CreateTodoList);
        group.MapPut(UpdateTodoList, "{id}");
        group.MapDelete(DeleteTodoList, "{id}");
    }

    public async Task<Ok<TodosVm>> GetTodoLists(ISender sender)
    {
        var vm = await sender.Send(new GetTodosQuery());

        return TypedResults.Ok(vm);
    }

    public async Task<Created<int>> CreateTodoList(ISender sender, CreateTodoListCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(TodoLists)}/{id}", id);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoList(ISender sender, int id, UpdateTodoListCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<NoContent> DeleteTodoList(ISender sender, int id)
    {
        await sender.Send(new DeleteTodoListCommand(id));

        return TypedResults.NoContent();
    }
}
