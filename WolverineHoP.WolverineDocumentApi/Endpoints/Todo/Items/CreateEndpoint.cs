using FluentValidation;
using JasperFx.Core;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;
using WolverineHoP.WolverineDocumentApi.Documents;

namespace WolverineHoP.WolverineDocumentApi.Endpoints.Todo.Items;

public static class CreateEndpoint
{
    public static ProblemDetails Validate(CreateTodoListItemRequest request, TodoList todoList)
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
        if (todoList.Items.Any(i => i.Description == description))
        {
            return new ProblemDetails
            {
                Detail = "List item must be unique",
                Status = StatusCodes.Status400BadRequest
            };
        }

        return WolverineContinue.NoProblems;
    }

    [WolverinePost("api/todo-list/{todoListId:guid}")]
    public static (IResult, IMartenOp) Post(
        CreateTodoListItemRequest request,
        [Document(Required = true)] TodoList todoList)
    {
        var newItem = new TodoListItem
        {
            Id = CombGuidIdGeneration.NewGuid(),
            Description = request.Description!
        };
        todoList.Items.Add(newItem);

        return (Results.Json(newItem.Id, statusCode: StatusCodes.Status201Created), MartenOps.Store(todoList));
    }
}

public class CreateTodoListItemRequest
{
    public string? Description { get; set; }

    public class Validator : AbstractValidator<CreateTodoListItemRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}