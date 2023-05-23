using System.ComponentModel.DataAnnotations;

namespace Todo.Data.Models;

public class TodoItem
{
    public Guid Id { get; set; }
    
    public DateTime Created { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public required string Text { get; set; }
    
    public DateTime? Completed { get; set; }
}