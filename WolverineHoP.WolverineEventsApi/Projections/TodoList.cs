using Marten.Events;
using WolverineHoP.WolverineEventsApi.Events;

namespace WolverineHoP.WolverineEventsApi.Projections;

public record TodoList(
    Guid Id, 
    string Title, 
    DateTimeOffset DateCreated, 
    bool Archived)
{
    public static TodoList Create(TodoListCreatedEvent @event, IEvent metadata)
    {
        return new TodoList(metadata.StreamId, @event.Title, metadata.Timestamp, false);
    }

    public static TodoList Apply(TodoListTitleUpdatedEvent @event, TodoList current)
    {
        return current with { Title = @event.Title };
    }

    public static TodoList Apply(TodoListArchivedEvent @event, TodoList current)
    {
        return current with { Archived = true };
    }
}