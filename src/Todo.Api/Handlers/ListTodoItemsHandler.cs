using Todo.Data.Models;

namespace Todo.Api.Handlers;


public record ListTodoItemsRequest(bool includeCompleted) : IRequest<IEnumerable<TodoItem>>;

public class ListTodoItemsHandler : IRequestHandler<ListTodoItemsRequest, IEnumerable<TodoItem>>
{
    private readonly ITodoRepository _todoRepository;


    public ListTodoItemsHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<IEnumerable<TodoItem>> Handle(ListTodoItemsRequest request, CancellationToken cancellationToken)
    {
        return await _todoRepository.List(request.includeCompleted);
    }
}