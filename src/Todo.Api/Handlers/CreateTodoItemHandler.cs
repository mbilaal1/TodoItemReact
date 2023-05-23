using System.ComponentModel.DataAnnotations;
using Todo.Data.Models;
namespace Todo.Api.Handlers;

public record CreateTodoItemRequest([Required(ErrorMessage = "The Text field is required!")] string Text) : IRequest<Guid>;

public class CreateTodoItemHandler : IRequestHandler<CreateTodoItemRequest, Guid>
{
    private readonly ITodoRepository _todoRepository;

    public CreateTodoItemHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Guid> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
    {

        var item = new TodoItem
        {
            Created = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            Text = request.Text.ToUpper()
        };

        var itemId = await _todoRepository.Create(item);
        return itemId;
    }
}