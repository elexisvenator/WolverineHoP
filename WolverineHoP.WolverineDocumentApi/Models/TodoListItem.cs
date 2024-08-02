namespace WolverineHoP.WolverineDocumentApi.Models;

public record TodoListItem(Guid Id, string Description, bool Checked)
{
    public static TodoListItem FromDocument(Documents.TodoListItem doc) =>
        new TodoListItem((Guid)doc.Id, (string)doc.Description, (bool)doc.Checked);
};