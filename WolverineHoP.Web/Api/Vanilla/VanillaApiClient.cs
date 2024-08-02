using OneOf.Types;
using OneOf;

namespace WolverineHoP.Web.Api.Vanilla;

using ModelErrors = Dictionary<string, List<string>>;

public class VanillaApiClient(HttpClient httpClient)
{
    public async Task<OneOf<Yes, ModelErrors>> SetupSchema(CancellationToken token = default)
    {
        var response = await httpClient.PostAsync("api/setup-schema", null, token);
        return await response.ParseSuccessOrError(token);
    }

    public ValueTask<List<TodoListSummary>> GetTodoLists(bool archived = false,
        CancellationToken cancellationToken = default)
    {
        return httpClient
            .GetFromJsonAsAsyncEnumerable<TodoListSummary>(
                $"/api/todo-list?archived={archived}", cancellationToken)
            .OfType<TodoListSummary>()
            .ToListAsync(cancellationToken);
    }

    public Task<TodoListDetail?> GetTodoList(long todoListId, CancellationToken cancellationToken = default)
    {
        return httpClient.GetFromJsonAsync<TodoListDetail>($"/api/todo-list/{todoListId}", cancellationToken);
    }

    public async Task<OneOf<long, ModelErrors>> CreateTodoList(string title, CancellationToken token = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/todo-list", new { Title = title }, token);
        return await response.ParseResponseOrError<long>(token);
    }

    public async Task<OneOf<Yes, ModelErrors>> UpdateTodoList(long todoListId, string title,
        CancellationToken token = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/todo-list/{todoListId}", new { Title = title }, token);
        return await response.ParseSuccessOrError(token);
    }

    public async Task<OneOf<Yes, ModelErrors>> ArchiveTodoList(long todoListId, CancellationToken token = default)
    {
        var response = await httpClient.PostAsync($"/api/todo-list/{todoListId}/archive", null, token);
        return await response.ParseSuccessOrError(token);
    }

    public async Task<OneOf<long, ModelErrors>> CreateTodoListItem(long todoListId, string description, CancellationToken token = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/todo-list/{todoListId}", new { Description = description }, token);
        return await response.ParseResponseOrError<long>(token);
    }

    public async Task<OneOf<Yes, ModelErrors>> CheckItem(long todoListId, long todoListItemId, CancellationToken token = default)
    {
        var response = await httpClient.PostAsync($"/api/todo-list/{todoListId}/{todoListItemId}/check", null, token);
        return await response.ParseSuccessOrError(token);
    }

    public async Task<OneOf<Yes, ModelErrors>> UncheckItem(long todoListId, long todoListItemId, CancellationToken token = default)
    {
        var response = await httpClient.PostAsync($"/api/todo-list/{todoListId}/{todoListItemId}/uncheck", null, token);
        return await response.ParseSuccessOrError(token);
    }
}