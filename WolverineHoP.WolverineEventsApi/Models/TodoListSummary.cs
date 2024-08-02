namespace WolverineHoP.WolverineEventsApi.Models;

public record TodoListSummary
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required DateTimeOffset DateCreated { get; init; }
    public required bool Archived { get; init; }
    public required int TotalItems { get; init; }
    public required int CompletedItems { get; init; }
}