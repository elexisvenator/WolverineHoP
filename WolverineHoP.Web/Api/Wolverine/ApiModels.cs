namespace WolverineHoP.Web.Api.Wolverine;

public class TodoListDetail
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public bool Archived { get; set; }
    public List<TodoListItem> Items { get; set; }
}

public class TodoListItem
{
    public Guid Id { get; init; }
    public string Description { get; init; }
    public bool Checked { get; set; }
}

public class TodoListSummary
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public DateTimeOffset DateCreated { get; init; }
    public bool Archived { get; init; }
    public int TotalItems { get; init; }
    public int CompletedItems { get; init; }
}