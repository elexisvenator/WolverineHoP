namespace WolverineHoP.WolverineEventsApi.Events;

// Used as a marker for the stream
public class TodoListStream;

public record TodoListCreatedEvent(string Title);
public record TodoListTitleUpdatedEvent(string Title);
public record TodoListArchivedEvent;
