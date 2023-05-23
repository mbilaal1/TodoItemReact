using Microsoft.EntityFrameworkCore;
using Todo.Data.Models;

namespace Todo.Data;

public class TodoRepository: ITodoRepository
{
    private readonly TodoContext _context;

    public TodoRepository(TodoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoItem>> List(bool includeCompleted)
    {   
        if (includeCompleted)
        {
            return await _context.TodoItems.ToArrayAsync();
        }
        else
        {
           return await _context.TodoItems.Where(item => item.Completed == null).ToArrayAsync();
        }

    }

    public async Task<Guid> Create(TodoItem newItem)
    {
        await _context.TodoItems.AddAsync(newItem);
        await _context.SaveChangesAsync();
        return newItem.Id;
    }

    public async Task<bool> MarkAsCompleted(Guid id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return false;
        }

        todoItem.Completed = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TodoItem?> GetItemById(Guid id)
    {
        var item = await _context.TodoItems.FindAsync(id);
        return item;
    }

    public async Task<bool> Update(TodoItem item)
    {
        _context.TodoItems.Update(item);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0; // it returns true, indicating that the update was successful. Otherwise, it returns false.
    }

    public async Task<IEnumerable<TodoItem>> SortTodoByMostRecent()
    {
        return await _context.TodoItems.OrderByDescending(item => item.Created).ToArrayAsync();
    }
}