using ProjectTemplate.Application.Common.Interfaces;

namespace ProjectTemplate.Application.TodoLists.Commands.PurgeTodoLists;

public record PurgeTodoListsCommand : IRequest;

public class PurgeTodoListsCommandHandler(IApplicationDbContext context) : IRequestHandler<PurgeTodoListsCommand>
{
    public async Task Handle(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        context.TodoLists.RemoveRange(context.TodoLists);

        await context.SaveChangesAsync(cancellationToken);
    }
}
