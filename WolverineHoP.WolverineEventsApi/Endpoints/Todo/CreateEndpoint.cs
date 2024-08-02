using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;
using WolverineHoP.WolverineEventsApi.Events;
using WolverineHoP.WolverineEventsApi.Projections;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo;

public static class CreateEndpoint
{
    public static async Task<ProblemDetails> Validate(CreateTodoListRequest request, IQuerySession session)
    {
        // title has already passed fluentValidation by this point.
        var title = request.Title!;
        var hasDuplicateName = await session.Query<TodoList>()
                .AnyAsync(x => x.Archived == false && x.Title.Equals(title));

        if (hasDuplicateName)
        {
            return new ProblemDetails
            {
                Detail = "List title must be unique", 
                Status = StatusCodes.Status400BadRequest
            };
        }

        return WolverineContinue.NoProblems;
    }

    [WolverinePost("api/todo-list")]
    public static (IResult, IMartenOp) Post(CreateTodoListRequest request)
    {
        var op = MartenOps.StartStream<TodoListStream>(new TodoListCreatedEvent(request.Title!));
        return (Results.Json(op.StreamId, statusCode: StatusCodes.Status201Created), op);
    }
}

public class CreateTodoListRequest
{
    public string? Title { get; set; }
    public class Validator : AbstractValidator<CreateTodoListRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty();
        }
    }
}