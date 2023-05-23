using Todo.Data.Models;

namespace Todo.Api.Handlers;

public record CompleteTodoItemRequest(Guid Id) : IRequest<bool>;

public class CompleteTodoItemHandler : IRequestHandler<CompleteTodoItemRequest, bool>
{
    private readonly ITodoRepository _todoRepository;

    public CompleteTodoItemHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<bool> Handle(CompleteTodoItemRequest request, CancellationToken cancellationToken)
    {


        var item = await _todoRepository.GetItemById(request.Id);
        if (item == null)
            return false;

        if (item.Completed.HasValue)
            return true; // Item is already completed

        item.Completed = DateTime.UtcNow;
        await _todoRepository.Update(item);
        return true;
    }
}
