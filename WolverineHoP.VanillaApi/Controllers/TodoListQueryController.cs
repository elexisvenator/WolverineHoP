using Microsoft.AspNetCore.Mvc;
using WolverineHoP.VanillaApi.Models;
using WolverineHoP.VanillaApi.Services;

namespace WolverineHoP.VanillaApi.Controllers;

[ApiController]
public class TodoListQueryController : ControllerBase
{
    private readonly ITodoListQueryService _todoListQueryService;

    public TodoListQueryController(ITodoListQueryService todoListQueryService)
    {
        _todoListQueryService = todoListQueryService;
    }

    /// <summary>
    /// List all todo lists
    /// </summary>
    [HttpGet("api/todo-list")]
    public IAsyncEnumerable<TodoListSummary> GetAllTodoLists(CancellationToken token, [FromQuery] bool archived = false)
    {
        return _todoListQueryService.GetAllTodoLists(archived, token);
    }

    /// <summary>
    /// Get a todo list and its items
    /// </summary>
    [HttpGet("api/todo-list/{todoListId:long}")]
    public async Task<ActionResult<TodoListDetail>> GetTodoList(long todoListId, CancellationToken token)
    {
        var todoList = await _todoListQueryService.GetTodoList(todoListId, token);
        if (todoList is null) return NotFound();

        return todoList;
    }
}