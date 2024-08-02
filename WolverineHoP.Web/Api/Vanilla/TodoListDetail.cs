namespace WolverineHoP.Web.Api.Vanilla;

public class TodoListDetail
{
    public long Id { get; init; }
    public string Title { get; init; }
    public DateTimeOffset DateCreated { get; init; }
    public bool Archived { get; init; }
    public List<TodoListItem> Items { get; init; }
}