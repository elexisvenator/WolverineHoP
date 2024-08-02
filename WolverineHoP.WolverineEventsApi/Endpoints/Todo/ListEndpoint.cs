using Marten;
using Wolverine.Http;
using WolverineHoP.WolverineEventsApi.Models;
using WolverineHoP.WolverineEventsApi.Projections;

namespace WolverineHoP.WolverineEventsApi.Endpoints.Todo;

public static class ListEndpoint
{
    [WolverineGet("api/todo-list")]
    public static async Task<IReadOnlyList<TodoListSummary>> Get(IQuerySession session, CancellationToken token, bool archived = false)
    {
        return await session.Query<TodoListWithCounts>()
            .Where(l => l.Archived == archived)
            .OrderBy(l => l.Title)
            .Select(l =>
                new TodoListSummary
                {
                    Id = l.Id,
                    Title = l.Title,
                    Archived = l.Archived,
                    DateCreated = l.DateCreated,
                    TotalItems = l.TotalItems,
                    CompletedItems = l.CheckedItems
                })
            .ToListAsync(token);
    }
}