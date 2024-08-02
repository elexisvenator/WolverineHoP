using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;
using WolverineHoP.WolverineEventsApi.Events;
using WolverineHoP.WolverineEventsApi.Projections;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo.Items;

public static class CreateEndpoint
{
    public static async Task<ProblemDetails> Validate(
        CreateTodoListItemRequest request, 
        TodoList todoList,
        IQuerySession session, 
        CancellationToken token)
    {
        if (todoList.Archived)
        {
            return new ProblemDetails
            {
                Detail = "Todo list is archived",
                Status = StatusCodes.Status400BadRequest
            };
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

    [WolverinePost("api/todo-list/{todoListId}")]
    public static async Task<(IResult, IMartenOp)> Post(
        CreateTodoListItemRequest request,
        [Aggregate] TodoList todoList,
        IQuerySession session,
        CancellationToken token)
    {
        var sequence = await session.Query<TodoListItem>().CountAsync(i => i.TodoListId == todoList.Id, token) + 1;
        var op = MartenOps.StartStream<TodoListItemStream>(
            new TodoListItemCreatedEvent(todoList.Id, sequence, request.Description!));

        return (Results.Json(op.StreamId, statusCode: StatusCodes.Status201Created), op);
    }
}

public class CreateTodoListItemRequest
{
    public string? Description { get; set; }

    public class Validator : AbstractValidator<CreateTodoListItemRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Description).NotNull().NotEmpty();
        }
    }
}