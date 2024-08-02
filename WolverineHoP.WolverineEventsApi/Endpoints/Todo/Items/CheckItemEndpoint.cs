using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using WolverineHoP.WolverineEventsApi.Events;
using WolverineHoP.WolverineEventsApi.Projections;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo.Items;

public static class CheckItemEndpoint
{
    public static async Task<ProblemDetails> Validate(
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

        return WolverineContinue.NoProblems;
    }

    [WolverinePost("api/todo-list/{todoListId:guid}/{todoListItemId:guid}/check"), EmptyResponse]
    public static TodoListItemCheckedEvent? Check([Aggregate] TodoListItem todoListItem)
    {
        return todoListItem.Checked ? null : new TodoListItemCheckedEvent(todoListItem.TodoListId);
    }

    [WolverinePost("api/todo-list/{todoListId:guid}/{todoListItemId:guid}/uncheck"), EmptyResponse]
    public static TodoListItemUncheckedEvent? Uncheck([Aggregate] TodoListItem todoListItem)
    {
        return todoListItem.Checked ? null : new TodoListItemUncheckedEvent(todoListItem.TodoListId);
    }
}