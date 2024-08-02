using System.Reflection;
using Marten;
using Oakton;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Marten;
using WolverineHoP.WolverineDocumentApi.Documents;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();

// Add services to the container.
builder.AddNpgsqlDataSource("postgresdb");
builder.Services.AddMarten(opts =>
    {
        opts.DatabaseSchemaName = "wolverine_document_api";
        opts.UseSystemTextJsonForSerialization();

        opts.RegisterDocumentType<TodoList>();
    })
    .UseNpgsqlDataSource()
    .UseLightweightSessions()
    .IntegrateWithWolverine();

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
