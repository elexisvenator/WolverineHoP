using WolverineHoP.WolverineDocumentApi.Documents;

namespace WolverineHoP.WolverineDocumentApi.Models;

public record TodoListDetail
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required DateTimeOffset DateCreated { get; init; }
    public required bool Archived { get; init; }
    public required IReadOnlyList<TodoListItem> Items { get; init; }

    internal static TodoListDetail FromDocument(TodoList document)
    {
        return new TodoListDetail
        {
            Id = document.Id,
            Title = document.Title,
            Items = document.Items.Select(TodoListItem.FromDocument).ToList(),
            Archived = document.Archived,
            DateCreated = document.CreatedDate
        };
    }
}