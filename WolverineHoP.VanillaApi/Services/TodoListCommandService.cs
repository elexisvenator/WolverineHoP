using Dapper;
using Npgsql;

namespace WolverineHoP.VanillaApi.Services;

public interface ITodoListCommandService
{
    Task<long> CreateTodoList(string title, CancellationToken token);
    Task ArchiveTodoList(long todoListId, CancellationToken token);
    Task EditTodoListName(long todoListId, string title, CancellationToken token);
    Task<long> CreateTodoListItem(long todoListId, string description, CancellationToken token);
    Task CheckTodoListItem(long todoListItemId, CancellationToken token);
    Task UncheckTodoListItem(long todoListItemId, CancellationToken token);
    Task EditTodoListItemDescription(long todoListItemId, string description, CancellationToken token);
}

public class TodoListCommandService : ITodoListCommandService
{
    private readonly NpgsqlDataSource _dataSource;

    public TodoListCommandService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateTodoList(string title, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = """
                           INSERT INTO vanilla_api.todo_list (title)
                           VALUES(@Title)
                           returning id
                           """;
        return await connection.ExecuteScalarAsync<long>(sql, new { Title = title });
    }

    public async Task ArchiveTodoList(long todoListId, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = """
                           update vanilla_api.todo_list
                           set archived = true
                           where id = @TodoListId
                           """;
        await connection.ExecuteAsync(sql, new { TodoListId = todoListId });
    }

    public async Task EditTodoListName(long todoListId, string title, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = """
                           update vanilla_api.todo_list
                           set title = @Title
                           where id = @TodoListId
                           """;
        await connection.ExecuteAsync(sql, new { TodoListId = todoListId, Title = title });
    }

    public async Task<long> CreateTodoListItem(long todoListId, string description, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = """
                           INSERT INTO vanilla_api.todo_list_item (todo_list_id, sequence_id, description)
                           select @TodoListId,
                                  coalesce((select max(sequence_id) from vanilla_api.todo_list_item where todo_list_id = @TodoListId), 0) + 1,
                                  @Description
                           returning id
                           """;
        return await connection.ExecuteScalarAsync<long>(sql, new { TodoListId = todoListId, Description = description });
    }

    public async Task CheckTodoListItem(long todoListItemId, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = """
                           update vanilla_api.todo_list_item
                           set checked = true
                           where id = @TodoListItemId
                           """;
        await connection.ExecuteAsync(sql, new { TodoListItemId = todoListItemId });
    }

    public async Task UncheckTodoListItem(long todoListItemId, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = """
                           update vanilla_api.todo_list_item
                           set checked = false
                           where id = @TodoListItemId
                           """;
        await connection.ExecuteAsync(sql, new { TodoListItemId = todoListItemId });
    }

    public async Task EditTodoListItemDescription(long todoListItemId, string description, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = """
                           update vanilla_api.todo_list_item
                           set description = @Description
                           where id = @TodoListItemId
                           """;
        await connection.ExecuteAsync(sql, new { TodoListItemId = todoListItemId, Description = description });
    }
}