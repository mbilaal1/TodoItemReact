using Microsoft.AspNetCore.Mvc;
using Todo.Api.Handlers;
using Todo.Data.Models;

namespace Todo.Api.Controllers;


[Route("api/todo")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ISender _sender;

    public TodoController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(bool includeCompleted)
    {
        var request = new ListTodoItemsRequest(includeCompleted);
        return Ok(await _sender.Send(request));
    }

    [HttpGet("mostrecentlycreated")]
    public async Task<IActionResult> ListoMostRecentTodo() =>
        Ok(await _sender.Send(new ListMostRecentTodoRequest()));
    


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemRequest request) =>
        Ok(await _sender.Send(request));
 
        

    [HttpPut("{id}/completed")]
    public async Task<IActionResult> MarkTodoItemAsCompleted(Guid id)
    {
        var request = new CompleteTodoItemRequest(id);
        return Ok(await _sender.Send(request));
    }
}