using Marten;
using Wolverine.Http;
using WolverineHoP.WolverineDocumentApi.Documents;
using WolverineHoP.WolverineDocumentApi.Models;

namespace WolverineHoP.WolverineDocumentApi.Endpoints.Todo;

public static class ListEndpoint
{
    [WolverineGet("api/todo-list")]
    public static async Task<IReadOnlyList<TodoListSummary>> Get(IQuerySession session, CancellationToken token, bool archived = false)
    {
        return await session.Query<TodoList>()
            .Where(l => l.Archived == archived)
            .OrderBy(l => l.Title)
            .Select(l =>
                new TodoListSummary
                {
                    Id = l.Id,
                    Title = l.Title,
                    Archived = l.Archived,
                    DateCreated = l.CreatedDate,
                    TotalItems = l.TotalItems,
                    CompletedItems = l.CompletedItems
                })
            .ToListAsync(token);
    }
}