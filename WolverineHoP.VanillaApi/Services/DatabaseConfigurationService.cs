using Npgsql;

namespace WolverineHoP.VanillaApi.Services;

public interface IDatabaseConfigurationService
{
    Task SeedData(CancellationToken token);
}

public class DatabaseConfigurationService : IDatabaseConfigurationService
{
    private readonly NpgsqlDataSource _dataSource;

    public DatabaseConfigurationService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task SeedData(CancellationToken token)
    {
        await ExecuteSql("CREATE SCHEMA IF NOT EXISTS vanilla_api;", token);
        await ExecuteSql("""
                         CREATE TABLE IF NOT EXISTS vanilla_api.todo_list (
                         	id int8 GENERATED ALWAYS AS IDENTITY NOT NULL,
                         	title text NOT NULL,
                         	date_created timestamptz DEFAULT now() NOT NULL,
                         	archived bool DEFAULT false NOT NULL,
                         	CONSTRAINT tasks_pk PRIMARY KEY (id)
                         );
                         """, token);
        await ExecuteSql("""
                         CREATE TABLE IF NOT EXISTS vanilla_api.todo_list_item (
                         	id int8 GENERATED ALWAYS AS IDENTITY NOT NULL,
                         	todo_list_id int8 NOT NULL,
                         	sequence_id int4 NOT NULL,
                         	description text NOT NULL,
                         	checked bool DEFAULT false NOT NULL,
                         	CONSTRAINT todo_list_item_pk PRIMARY KEY (id),
                         	CONSTRAINT todo_list_item_unique UNIQUE (todo_list_id, sequence_id),
                         	CONSTRAINT todo_list_item_todo_list_fk FOREIGN KEY (todo_list_id) REFERENCES vanilla_api.todo_list(id) ON DELETE CASCADE
                         );
                         """, token);
    }

    private async Task ExecuteSql(string sql, CancellationToken token)
    {
        await using var command = _dataSource.CreateCommand(sql);
        await command.ExecuteNonQueryAsync(token);
    }
}