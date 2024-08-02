using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WolverineHoP.VanillaApi.Services;

namespace WolverineHoP.VanillaApi.Controllers;

[ApiController]
public class TodoListCommandController : ControllerBase
{
    private readonly ITodoListQueryService _todoListQueryService;
    private readonly ITodoListCommandService _todoListCommandService;

    public TodoListCommandController(ITodoListQueryService todoListQueryService, ITodoListCommandService todoListCommandService)
    {
        _todoListQueryService = todoListQueryService;
        _todoListCommandService = todoListCommandService;
    }

    /// <summary>
    /// Create a new todo list
    /// </summary>
    [HttpPost("api/todo-list")]
    public async Task<ActionResult<long>> Create(CreateTodoListRequest model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _todoListQueryService.CheckTodoListWithTitleExists(model.Title!, token))
        {
            ModelState.AddModelError(nameof(model.Title), "List title must be unique");
            return BadRequest(ModelState);
        }

        var todoListId = await _todoListCommandService.CreateTodoList(model.Title!, token);
        return StatusCode(StatusCodes.Status201Created, todoListId);
    }

    public class CreateTodoListRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string? Title { get; set; }
    }

    /// <summary>
    /// Update todo list
    /// </summary>
    [HttpPut("api/todo-list/{todoListId:long}")]
    public async Task<ActionResult> Update(long todoListId, UpdateTodoListRequest model, CancellationToken token)
    {
        var todoList = await _todoListQueryService.GetTodoList(todoListId, token);
        if (todoList is null)
        {
            return NotFound();
        }

        if (todoList.Archived)
        {
            return BadRequest("Todo list is archived");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (todoList.Title.Equals(model.Title))
        {
            // no change
            return NoContent();
        }


        if (await _todoListQueryService.CheckTodoListWithTitleExists(model.Title!, token))
        {
            ModelState.AddModelError(nameof(model.Title), "List title must be unique");
            return BadRequest(ModelState);
        }

        await _todoListCommandService.EditTodoListName(todoListId, model.Title!, token);
        return Ok();
    }

    public class UpdateTodoListRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string? Title { get; set; }
    }

    /// <summary>
    /// Archive todo list
    /// </summary>
    [HttpPost("api/todo-list/{todoListId:long}/archive")]
    public async Task<ActionResult> Archive(long todoListId, CancellationToken token)
    {
        var todoList = await _todoListQueryService.GetTodoList(todoListId, token);
        if (todoList is null)
        {
            return NotFound();
        }

        if (todoList.Archived)
        {
            // no change
            return NoContent();
        }

        await _todoListCommandService.ArchiveTodoList(todoListId, token);
        return Ok();
    }
}