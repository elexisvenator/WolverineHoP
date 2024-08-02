using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using WolverineHoP.WolverineEventsApi.Events;
using WolverineHoP.WolverineEventsApi.Projections;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo.Items;

public static class UpdateDescriptionEndpoint
{
    public static async Task<ProblemDetails> Validate(
        UpdateTodoListItemDescriptionRequest request,
        TodoListItem todoListItem,
        Guid todoListId,
        IDocumentSession session,
        CancellationToken token)
    {
        if (todoListItem.TodoListId != todoListId)
        {
            // item doesn't exist
            return new ProblemDetails { Status = StatusCodes.Status404NotFound };
        }

        var todoList = await session.Events.FetchForWriting<TodoList>(todoListId, token);
        if (todoList.Aggregate is null)
        {
            // list doesn't exist, shouldn't happen
            return new ProblemDetails { Status = StatusCodes.Status404NotFound };
        }

        if (todoList.Aggregate.Archived)
        {
            return (
                new ProblemDetails
                {
                    Detail = "Todo list is archived",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        // description has already passed fluentValidation by this point.
        var description = request.Description!;
        if (await session.Query<TodoListItem>().AnyAsync(i => i.TodoListId == todoList.Id && i.Description.Equals(description), token))
        {
            return new ProblemDetails
            {
                Detail = "List item must be unique",
                Status = StatusCodes.Status400BadRequest
            };
        }

        return WolverineContinue.NoProblems;
    }

    [WolverinePut("api/todo-list/{todoListId:guid}/{todoListItemId:guid}"), EmptyResponse]
    public static TodoListItemDescriptionUpdatedEvent? Put(
        UpdateTodoListItemDescriptionRequest request, 
        [Aggregate] TodoListItem todoListItem)
    {
        var description = request.Description!;
        return todoListItem.Description.Equals(description) 
            ? null 
            : new TodoListItemDescriptionUpdatedEvent(todoListItem.TodoListId, description);
    }
}

public class UpdateTodoListItemDescriptionRequest
{
    public string? Description { get; set; }

    public class Validator : AbstractValidator<UpdateTodoListItemDescriptionRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Description).NotNull().NotEmpty();
        }
    }
}