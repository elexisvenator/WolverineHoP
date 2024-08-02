using Marten;
using Wolverine.Http;
using Wolverine.Http.Marten;
using WolverineHoP.WolverineEventsApi.Models;
using WolverineHoP.WolverineEventsApi.Projections;
using TodoListItem = WolverineHoP.WolverineEventsApi.Projections.TodoListItem;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo;

public static class GetEndpoint
{
    [WolverineGet("api/todo-list/{todoListId:guid}")]
    public static async Task<TodoListDetail> Get([Aggregate] TodoList todoList, IQuerySession session)
    {
        var items = await session.Query<TodoListItem>()
            .Where(i => i.TodoListId == todoList.Id)
            .OrderBy(i => i.Sequence)
            .ToListAsync();
        return TodoListDetail.FromDocument(todoList, items);
    }
}