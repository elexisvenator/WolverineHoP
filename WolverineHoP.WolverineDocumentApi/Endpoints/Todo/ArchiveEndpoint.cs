using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;
using WolverineHoP.WolverineDocumentApi.Documents;

namespace WolverineHoP.WolverineDocumentApi.Endpoints.Todo;

public static class ArchiveEndpoint
{
    [WolverinePost("api/todo-list/{todoListId:guid}/archive")]
    public static IMartenOp Archive([Document(Required = true)] TodoList todoList)
    {
        return todoList.Archived
            ? MartenOps.Nothing()
            : MartenOps.Store(todoList with { Archived = true });
    }
}