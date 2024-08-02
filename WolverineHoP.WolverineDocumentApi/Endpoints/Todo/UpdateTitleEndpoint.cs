using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;
using WolverineHoP.WolverineDocumentApi.Documents;

namespace WolverineHoP.WolverineDocumentApi.Endpoints.Todo;

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

    [WolverinePut("api/todo-list/{todoListId:guid}")]
    public static IMartenOp Put(UpdateTodoListTitleRequest request, [Document(Required = true)] TodoList todoList)
    {
        return todoList.Title.Equals(request.Title!)
            ? MartenOps.Nothing()
            : MartenOps.Store(todoList with { Title = request.Title! });
    }
}

public class UpdateTodoListTitleRequest
{
    public string? Title { get; set; }

    public class Validator : AbstractValidator<UpdateTodoListTitleRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}