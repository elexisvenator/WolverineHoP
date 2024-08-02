namespace WolverineHoP.WolverineEventsApi.Events;

// Used as a marker for the stream
public class TodoListItemStream;

public record TodoListItemCreatedEvent(Guid TodoListId, int Sequence, string Description);
public record TodoListItemDescriptionUpdatedEvent(Guid TodoListId, string Description);
public record TodoListItemCheckedEvent(Guid TodoListId);
public record TodoListItemUncheckedEvent(Guid TodoListId);
