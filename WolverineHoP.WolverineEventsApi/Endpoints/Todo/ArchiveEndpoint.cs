using Wolverine.Http;
using Wolverine.Http.Marten;
using WolverineHoP.WolverineEventsApi.Events;
using WolverineHoP.WolverineEventsApi.Projections;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo;

public static class ArchiveEndpoint
{
    [WolverinePost("api/todo-list/{todoListId:guid}/archive"), EmptyResponse]
    public static TodoListArchivedEvent? Archive([Aggregate] TodoList todoList)
    {
        return todoList.Archived
            ? null
            : new TodoListArchivedEvent();
    }
}