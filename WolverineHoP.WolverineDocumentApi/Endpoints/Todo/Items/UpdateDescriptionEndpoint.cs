using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;
using WolverineHoP.WolverineDocumentApi.Documents;

namespace WolverineHoP.WolverineDocumentApi.Endpoints.Todo.Items;

public static class UpdateDescriptionEndpoint
{
    public static (ProblemDetails validationErrors, TodoListItem? currentTodoListItem) Validate(
        UpdateTodoListItemDescriptionRequest request,
        TodoList todoList,
        Guid todoListItemId)
    {
        // description has already passed fluentValidation by this point.
        var description = request.Description!;

        if (todoList.Items.SingleOrDefault(i => i.Id == todoListItemId) is not {} todoListItem)
        {
            // item doesn't exist
            // item doesn't exist
            return (
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound
                },
                null);
        }

        if (todoList.Archived)
        {
            return (
                new ProblemDetails
                {
                    Detail = "Todo list is archived",
                    Status = StatusCodes.Status400BadRequest
                },
                null);
        }

        if (todoList.Items.Any(i => i.Description.Equals(description) && !i.Id.Equals(todoListItemId)))
        {
            return (
                new ProblemDetails
                {
                    Detail = "List item description must be unique",
                    Status = StatusCodes.Status400BadRequest
                },
                null);
        }

        return (WolverineContinue.NoProblems, todoListItem);
    }

    [WolverinePut("api/todo-list/{todoListId:guid}/{todoListItemId:guid}")]
    public static IMartenOp Put(
        UpdateTodoListItemDescriptionRequest request, 
        [Document(Required = true)] TodoList todoList, 
        TodoListItem todoListItem)
    {
        if (todoListItem.Description.Equals(request.Description!))
        {
            return MartenOps.Nothing();
        }

        todoListItem.Description = request.Description!;
        return MartenOps.Store(todoList);
    }
}

public class UpdateTodoListItemDescriptionRequest
{
    public string? Description { get; set; }

    public class Validator : AbstractValidator<UpdateTodoListItemDescriptionRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}