using Todo.Data.Models;

namespace Todo.Data;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> List(bool includeCompleted);
    Task<IEnumerable<TodoItem>> SortTodoByMostRecent();

    Task<Guid> Create(TodoItem newItem);

    Task<TodoItem?> GetItemById(Guid id);

    Task<bool> Update(TodoItem item);

}