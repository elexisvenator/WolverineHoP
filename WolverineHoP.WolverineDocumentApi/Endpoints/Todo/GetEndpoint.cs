using Wolverine.Http;
using Wolverine.Http.Marten;
using WolverineHoP.WolverineDocumentApi.Documents;
using WolverineHoP.WolverineDocumentApi.Models;

namespace WolverineHoP.WolverineDocumentApi.Endpoints.Todo;

public static class GetEndpoint
{
    [WolverineGet("api/todo-list/{todoListId:guid}")]
    public static TodoListDetail Get([Document(Required = true)] TodoList todoList)
    {
        return TodoListDetail.FromDocument(todoList);
    }
}