namespace WolverineHoP.VanillaApi.Models;

public record TodoListItem
{
    public long Id { get; init; }
    public int SequenceId { get; init; }
    public string Description { get; init; }
    public bool Checked { get; init; }
}