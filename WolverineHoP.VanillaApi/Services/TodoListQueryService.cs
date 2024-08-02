using System.Runtime.CompilerServices;
using Dapper;
using Npgsql;
using WolverineHoP.VanillaApi.Models;

namespace WolverineHoP.VanillaApi.Services;

public interface ITodoListQueryService
{
    IAsyncEnumerable<TodoListSummary> GetAllTodoLists(bool archived, CancellationToken token);
    Task<TodoListDetail?> GetTodoList(long todoListId, CancellationToken token);
    Task<bool> CheckTodoListWithTitleExists(string title, CancellationToken token);
}

public class TodoListQueryService : ITodoListQueryService
{
    private readonly NpgsqlDataSource _dataSource;

    public TodoListQueryService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<TodoListSummary> GetAllTodoLists(bool archived, [EnumeratorCancellation] CancellationToken token)
    {
        const string sql = """
                           select tl.id as "Id",
                           	   tl.title as "Title",
                           	   tl.date_created as "DateCreated",
                           	   tl.archived as "Archived",
                           	   count(1) filter (where tli.id is not null) as "TotalItems",
                           	   count(1) filter (where tli.checked) as "CompletedItems"
                           from vanilla_api.todo_list tl 
                           left join vanilla_api.todo_list_item tli on tli.todo_list_id = tl.id 
                           where tl.archived = @Archived
                           group by 1
                           order by tl.title
                           """;
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        await foreach (var summary in connection.QueryUnbufferedAsync<TodoListSummary>(sql, new { Archived = archived }).WithCancellation(token))
        {
            yield return summary;
        } 
    }

    private record TodoListResponse
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public DateTime DateCreated { get; init; }
        public bool Archived { get; init; }
    }

    public async Task<TodoListDetail?> GetTodoList(long todoListId, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql1 = """
                            select tl.id as "Id",
                            	   tl.title as "Title",
                            	   tl.date_created as "DateCreated",
                            	   tl.archived as "Archived"
                            from vanilla_api.todo_list tl 
                            where tl.id  = @TodoListId
                            """;
        const string sql2 = """
                            select tli.id as "Id",
                            	   tli.sequence_id as "SequenceId",
                            	   tli.description as "Description",
                            	   tli.checked as "Checked"
                            from vanilla_api.todo_list_item tli
                            where tli.todo_list_id  = @TodoListId
                            """;
        var todoList =
            await connection.QuerySingleOrDefaultAsync<TodoListResponse>(sql1, new { TodoListId = todoListId });
        if (todoList is null)
        {
            return null;
        }

        var items = await connection.QueryAsync<TodoListItem>(sql2, new { TodoListId = todoListId });
        return new TodoListDetail
        {
            Id = todoList.Id, 
            Title = todoList.Title, 
            DateCreated = todoList.DateCreated, 
            Archived = todoList.Archived,
            Items = items.OrderBy(x => x.SequenceId).ToList()
    };
}

    public async Task<bool> CheckTodoListWithTitleExists(string title, CancellationToken token)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(token);
        const string sql = "select exists (select 1 from vanilla_api.todo_list where title = @Title and archived is false)";
        return await connection.ExecuteScalarAsync<bool>(sql, new { Title = title });
    }
}