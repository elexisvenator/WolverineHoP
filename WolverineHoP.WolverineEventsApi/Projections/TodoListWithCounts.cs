using Marten.Events;
using Marten.Events.Projections;
using WolverineHoP.WolverineEventsApi.Events;

namespace WolverineHoP.WolverineEventsApi.Projections;

public record TodoListWithCounts
{
    public Guid Id { get; init; }
    public string Title { get; init; } = "";
    public DateTimeOffset DateCreated { get; init; }
    public bool Archived { get; init; }
    public int TotalItems { get; init; }
    public int CheckedItems { get; init; }

    public class Projector : MultiStreamProjection<TodoListWithCounts, Guid>
    {
        public Projector()
        {
            Identity<IEvent<TodoListCreatedEvent>>(e => e.StreamId);
            Identity<IEvent<TodoListTitleUpdatedEvent>>(e => e.StreamId);
            Identity<IEvent<TodoListArchivedEvent>>(e => e.StreamId);


            Identity<TodoListItemCreatedEvent>(e => e.TodoListId);
            Identity<TodoListItemUncheckedEvent>(e => e.TodoListId);
            Identity<TodoListItemCheckedEvent>(e => e.TodoListId);
        }

        public static TodoListWithCounts Apply(TodoListCreatedEvent @event, IEvent metadata, TodoListWithCounts current)
        {
            return current with { Title = @event.Title, DateCreated = metadata.Timestamp };
        }

        public static TodoListWithCounts Apply(TodoListTitleUpdatedEvent @event, TodoListWithCounts current)
        {
            return current with { Title = @event.Title };
        }

        public static TodoListWithCounts Apply(TodoListArchivedEvent @event, TodoListWithCounts current)
        {
            return current with { Archived = true };
        }


        public static TodoListWithCounts Apply(TodoListItemCreatedEvent @event, TodoListWithCounts current)
        {
            return current with { TotalItems = current.TotalItems + 1 };
        }

        public static TodoListWithCounts Apply(TodoListItemCheckedEvent @event, TodoListWithCounts current)
        {
            return current with { CheckedItems = current.CheckedItems + 1 };
        }

        public static TodoListWithCounts Apply(TodoListItemUncheckedEvent @event, TodoListWithCounts current)
        {
            return current with { CheckedItems = current.CheckedItems - 1 };
        }
    }
}