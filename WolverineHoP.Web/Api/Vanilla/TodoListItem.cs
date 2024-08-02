namespace WolverineHoP.Web.Api.Vanilla;

public class TodoListItem
{
    public long Id { get; init; }
    public int SequenceId { get; init; }
    public string Description { get; init; }
    public bool Checked { get; set; }
}