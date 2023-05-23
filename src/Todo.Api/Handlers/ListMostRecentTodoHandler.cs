using Todo.Data.Models;

namespace Todo.Api.Handlers;


public record ListMostRecentTodoRequest() : IRequest<IEnumerable<TodoItem>>;

public class ListMostRecentTodoHandler : IRequestHandler<ListMostRecentTodoRequest, IEnumerable<TodoItem>>
{
    private readonly ITodoRepository _todoRepository;


    public ListMostRecentTodoHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<IEnumerable<TodoItem>> Handle(ListMostRecentTodoRequest request, CancellationToken cancellationToken)
    {
        return await _todoRepository.SortTodoByMostRecent();
    }
}