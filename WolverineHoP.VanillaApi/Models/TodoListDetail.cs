namespace WolverineHoP.VanillaApi.Models;

public record TodoListDetail
{
    public long Id { get; init; }
    public string Title { get; init; }
    public DateTimeOffset DateCreated { get; init; }
    public bool Archived { get; init; }
    public IReadOnlyList<TodoListItem> Items { get; init; }
}