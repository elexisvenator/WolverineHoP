using System.Reflection;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Storage;
using Oakton;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Marten;
using WolverineHoP.WolverineEventsApi.Events;
using WolverineHoP.WolverineEventsApi.Projections;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();

// Add services to the container.
builder.AddNpgsqlDataSource("postgresdb");
builder.Services.AddMarten(opts =>
    {
        opts.DatabaseSchemaName = "wolverine_events_api";
        opts.UseSystemTextJsonForSerialization();

        opts.Policies.AllDocumentsAreMultiTenanted();

        // not necessary, but stops slow discovery at runtime
        opts.Events.AddEventType<TodoListCreatedEvent>();
        opts.Events.AddEventType<TodoListTitleUpdatedEvent>();
        opts.Events.AddEventType<TodoListArchivedEvent>();
        opts.Events.AddEventType<TodoListItemCreatedEvent>();
        opts.Events.AddEventType<TodoListItemDescriptionUpdatedEvent>();
        opts.Events.AddEventType<TodoListItemCheckedEvent>();
        opts.Events.AddEventType<TodoListItemUncheckedEvent>();

        opts.Projections.Snapshot<TodoList>(SnapshotLifecycle.Inline);
        opts.Projections.Snapshot<TodoListItem>(SnapshotLifecycle.Inline);
        opts.Projections.Add<TodoListWithCounts.Projector>(ProjectionLifecycle.Async);

        opts.Events.TenancyStyle = TenancyStyle.Conjoined;
    })
    .UseNpgsqlDataSource()
    .UseLightweightSessions()
    .IntegrateWithWolverine()
    .AddAsyncDaemon(DaemonMode.Solo);

builder.Host.UseWolverine(opts =>
{
    opts.UseFluentValidation();
    opts.UseSystemTextJsonForSerialization();
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});
var app = builder.Build();

//app.MapDefaultEndpoints();
app.MapWolverineEndpoints(opts =>
{
    // Opting into the Fluent Validation middleware from
    // Wolverine.Http.FluentValidation
    opts.UseFluentValidationProblemDetailMiddleware();
    opts.TenantId.IsQueryStringValue("tenant");
    opts.TenantId.AssertExists();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// run command
await app.RunOaktonCommands(args);