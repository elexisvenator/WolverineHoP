namespace WolverineHoP.WolverineDocumentApi.Documents;

public record TodoList
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public DateTimeOffset CreatedDate { get; init; } = DateTimeOffset.UtcNow;
    public bool Archived { get; init; } = false;
    public List<TodoListItem> Items { get; init; } = [];

    // Computed column that is persisted to the db
    public int TotalItems
    {
        get => Items.Count;
        // ReSharper disable once ValueParameterNotUsed
        init { }
    }

    // Computed column that is persisted to the db
    public int CompletedItems
    {
        get => Items.Count(i => i.Checked);
        // ReSharper disable once ValueParameterNotUsed
        init { }
    }
}

public record TodoListItem
{
    public required Guid Id { get; init; }
    public required string Description { get; set; }
    public bool Checked { get; set; } = false;
}
