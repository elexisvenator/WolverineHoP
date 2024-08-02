namespace WolverineHoP.VanillaApi.Models;

public record TodoListSummary
{
    public long Id { get; init; }
    public string Title { get; init; }
    public DateTimeOffset DateCreated { get; init; }
    public bool Archived { get; init; }
    public int TotalItems { get; init; }
    public int CompletedItems { get; init; }
}