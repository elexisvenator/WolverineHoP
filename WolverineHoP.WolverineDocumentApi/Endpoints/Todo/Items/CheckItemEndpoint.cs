using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;
using WolverineHoP.WolverineDocumentApi.Documents;

namespace WolverineHoP.WolverineDocumentApi.Endpoints.Todo.Items;

public static class CheckItemEndpoint
{
    public static (ProblemDetails validationErrors, TodoListItem? currentTodoListItem) Validate(
        TodoList todoList,
        Guid todoListItemId)
    {
        if (todoList.Items.SingleOrDefault(i => i.Id == todoListItemId) is not { } todoListItem)
        {
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

        return (WolverineContinue.NoProblems, todoListItem);
    }

    [WolverinePost("api/todo-list/{todoListId:guid}/{todoListItemId:guid}/check")]
    public static IMartenOp Check(
        [Document(Required = true)] TodoList todoList,
        [Required, NotBody] TodoListItem todoListItem)
    {
        if (todoListItem.Checked) return MartenOps.Nothing();
        
        todoListItem.Checked = true;
        return MartenOps.Store(todoList);

    }

    [WolverinePost("api/todo-list/{todoListId:guid}/{todoListItemId:guid}/uncheck")]
    public static IMartenOp Uncheck(
        [Document(Required = true)] TodoList todoList,
        [NotBody] TodoListItem todoListItem)
    {
        if (!todoListItem.Checked) return MartenOps.Nothing();

        todoListItem.Checked = false;
        return MartenOps.Store(todoList);
    }
}