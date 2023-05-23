using Microsoft.EntityFrameworkCore;
using Todo.Data.Models;

namespace Todo.Data;

public class TodoContext : DbContext
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }
}