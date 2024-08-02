using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WolverineHoP.VanillaApi.Services;

namespace WolverineHoP.VanillaApi.Controllers;

[ApiController]
public class TodoListItemCommandController : ControllerBase
{
    private readonly ITodoListQueryService _todoListQueryService;
    private readonly ITodoListCommandService _todoListCommandService;

    public TodoListItemCommandController(ITodoListQueryService todoListQueryService, ITodoListCommandService todoListCommandService)
    {
        _todoListQueryService = todoListQueryService;
        _todoListCommandService = todoListCommandService;
    }

    /// <summary>
    /// Create a new todo list item
    /// </summary>
    [HttpPost("api/todo-list/{todoListId:long}")]
    public async Task<ActionResult<long>> Create(long todoListId, CreateTodoListItemRequest model, CancellationToken token)
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

        if (todoList.Items.Any(i => i.Description.Equals(model.Description)))
        {
            ModelState.AddModelError(nameof(model.Description), "List item must be unique");
            return BadRequest(ModelState);
        }

        var todoListItemId = await _todoListCommandService.CreateTodoListItem(todoListId, model.Description!, token);
        return StatusCode(StatusCodes.Status201Created, todoListItemId);
    }

    public class CreateTodoListItemRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Update a todo list item
    /// </summary>
    [HttpPut("api/todo-list/{todoListId:long}/{todoListItemId:long}")]
    public async Task<ActionResult> Update(long todoListId, long todoListItemId, UpdateTodoListItemRequest model, CancellationToken token)
    {
        var todoList = await _todoListQueryService.GetTodoList(todoListId, token);
        var todoListItem = todoList?.Items.FirstOrDefault(i => i.Id == todoListItemId);
        if (todoList is null || todoListItem is null)
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

        if (todoListItem.Description.Equals(model.Description))
        {
            // no change
            return NoContent();
        }

        if (todoList.Items.Any(i => i.Description.Equals(model.Description)))
        {
            ModelState.AddModelError(nameof(model.Description), "List item description must be unique");
            return BadRequest(ModelState);
        }

        await _todoListCommandService.EditTodoListItemDescription(todoListItemId, model.Description!, token);
        return Ok();
    }

    public class UpdateTodoListItemRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Check a todo list item
    /// </summary>
    [HttpPost("api/todo-list/{todoListId:long}/{todoListItemId:long}/check")]
    public async Task<ActionResult> CheckItem(long todoListId, long todoListItemId, CancellationToken token)
    {
        var todoList = await _todoListQueryService.GetTodoList(todoListId, token);
        var todoListItem = todoList?.Items.FirstOrDefault(i => i.Id == todoListItemId);
        if (todoList is null || todoListItem is null)
        {
            return NotFound();
        }

        if (todoList.Archived)
        {
            return BadRequest("Todo list is archived");
        }

        if (todoListItem.Checked)
        {
            // no change
            return NoContent();
        }

        await _todoListCommandService.CheckTodoListItem(todoListItemId, token);
        return Ok();
    }

    /// <summary>
    /// Uncheck a todo list item
    /// </summary>
    [HttpPost("api/todo-list/{todoListId:long}/{todoListItemId:long}/uncheck")]
    public async Task<ActionResult> UncheckItem(long todoListId, long todoListItemId, CancellationToken token)
    {
        var todoList = await _todoListQueryService.GetTodoList(todoListId, token);
        var todoListItem = todoList?.Items.FirstOrDefault(i => i.Id == todoListItemId);
        if (todoList is null || todoListItem is null)
        {
            return NotFound();
        }

        if (todoList.Archived)
        {
            return BadRequest("Todo list is archived");
        }

        if (!todoListItem.Checked)
        {
            // no change
            return NoContent();
        }

        await _todoListCommandService.UncheckTodoListItem(todoListItemId, token);
        return Ok();
    }
}