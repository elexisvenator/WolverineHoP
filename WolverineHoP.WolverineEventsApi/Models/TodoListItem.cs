namespace WolverineHoP.WolverineEventsApi.Models;

public record TodoListItem(Guid Id, string Description, bool Checked)
{
    public static TodoListItem FromDocument(Projections.TodoListItem doc) =>
        new TodoListItem(doc.Id, doc.Description, doc.Checked);
};