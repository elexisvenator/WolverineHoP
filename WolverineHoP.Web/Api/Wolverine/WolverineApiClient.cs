using OneOf;
using OneOf.Types;

namespace WolverineHoP.Web.Api.Wolverine;
using IdOrError = OneOf<Guid, Dictionary<string, List<string>>>;
using SuccessOrError = OneOf<Yes, Dictionary<string, List<string>>>;

public abstract class WolverineApiClient(HttpClient httpClient)
{
    public ValueTask<List<TodoListSummary>> GetTodoLists(bool archived = false, int? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        return httpClient
            .GetFromJsonAsAsyncEnumerable<TodoListSummary>(
                $"/api/todo-list?archived={archived}&tenant={tenantId}", 
                cancellationToken)
            .OfType<TodoListSummary>()
            .ToListAsync(cancellationToken);
    }

    public Task<TodoListDetail?> GetTodoList(
        Guid todoListId, 
        int? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        return httpClient.GetFromJsonAsync<TodoListDetail>(
            $"/api/todo-list/{todoListId}?tenant={tenantId}",
            cancellationToken);
    }

    public async Task<IdOrError> CreateTodoList(
        string title, 
        int? tenantId = null,
        CancellationToken token = default)
    {
        var response =
            await httpClient.PostAsJsonAsync($"/api/todo-list?tenant={tenantId}", new { Title = title }, token);
        return await response.ParseResponseOrError<Guid>(token);
    }

    public async Task<SuccessOrError> UpdateTodoList(
        Guid todoListId, 
        string title,
        int? tenantId = null, 
        CancellationToken token = default)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"/api/todo-list/{todoListId}?tenant={tenantId}",
            new { Title = title }, 
            token);
        return await response.ParseSuccessOrError(token);
    }

    public async Task<SuccessOrError> ArchiveTodoList(
        Guid todoListId,
        int? tenantId = null, 
        CancellationToken token = default)
    {
        var response =
            await httpClient.PostAsync($"/api/todo-list/{todoListId}/archive?tenant={tenantId}", null, token);
        return await response.ParseSuccessOrError(token);
    }

    public async Task<IdOrError> CreateTodoListItem(
        Guid todoListId,
        string description, 
        int? tenantId = null, 
        CancellationToken token = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"/api/todo-list/{todoListId}?tenant={tenantId}",
            new { Description = description }, 
            token);
        return await response.ParseResponseOrError<Guid>(token);
    }

    public async Task<SuccessOrError> CheckItem(
        Guid todoListId, 
        Guid todoListItemId,
        int? tenantId = null, 
        CancellationToken token = default)
    {
        var response =
            await httpClient.PostAsync(
                $"/api/todo-list/{todoListId}/{todoListItemId}/check?tenant={tenantId}", 
                null,
                token);
        return await response.ParseSuccessOrError(token);
    }

    public async Task<SuccessOrError> UncheckItem(
        Guid todoListId, 
        Guid todoListItemId,
        int? tenantId = null, 
        CancellationToken token = default)
    {
        var response =
            await httpClient.PostAsync(
                $"/api/todo-list/{todoListId}/{todoListItemId}/uncheck?tenant={tenantId}", 
                null,
                token);
        return await response.ParseSuccessOrError(token);
    }
}