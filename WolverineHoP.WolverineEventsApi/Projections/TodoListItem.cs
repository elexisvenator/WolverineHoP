using Marten.Events;
using Marten.Schema;
using WolverineHoP.WolverineEventsApi.Events;

namespace WolverineHoP.WolverineEventsApi.Projections;

public record TodoListItem(
    Guid Id, 
    [property: DuplicateField] Guid TodoListId,
    int Sequence,
    string Description, 
    bool Checked)
{
    public static TodoListItem Create(TodoListItemCreatedEvent @event, IEvent metadata)
    {
        return new TodoListItem(metadata.StreamId, @event.TodoListId, @event.Sequence, @event.Description, false);
    }

    public static TodoListItem Apply(TodoListItemDescriptionUpdatedEvent @event, TodoListItem current)
    {
        return current with { Description = @event.Description };
    }

    public static TodoListItem Apply(TodoListItemCheckedEvent @event, TodoListItem current)
    {
        return current with { Checked = true };
    }

    public static TodoListItem Apply(TodoListItemUncheckedEvent @event, TodoListItem current)
    {
        return current with { Checked = false };
    }
}