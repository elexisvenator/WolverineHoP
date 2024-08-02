using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using WolverineHoP.WolverineEventsApi.Events;
using WolverineHoP.WolverineEventsApi.Projections;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo;

public static class UpdateTitleEndpoint
{
    public static async Task<ProblemDetails> Validate(
        UpdateTodoListTitleRequest request,
        TodoList todoList,
        IQuerySession session,
        CancellationToken token)
    {
        // title has already passed fluentValidation by this point.
        var title = request.Title!;
        var hasDuplicateName = !todoList.Title.Equals(title)
                               && await session.Query<TodoList>()
                                   .AnyAsync(x => x.Archived == false
                                                  && x.Title.Equals(title)
                                                  && x.Id != todoList.Id,
                                       token);
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

    [WolverinePut("api/todo-list/{todoListId:guid}"), EmptyResponse]
    public static TodoListTitleUpdatedEvent? Put(UpdateTodoListTitleRequest request, [Aggregate] TodoList todoList)
    {
        var title = request.Title!;
        return todoList.Title.Equals(title)
            ? null
            : new TodoListTitleUpdatedEvent(title);
    }
}

public class UpdateTodoListTitleRequest
{
    public string? Title { get; set; }

    public class Validator : AbstractValidator<UpdateTodoListTitleRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty();
        }
    }
}