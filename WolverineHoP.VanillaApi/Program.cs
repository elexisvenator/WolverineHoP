using System.Reflection;
using WolverineHoP.ServiceDefaults;
using WolverineHoP.VanillaApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddNpgsqlDataSource("postgresdb");

builder.Services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();
builder.Services.AddScoped<ITodoListCommandService, TodoListCommandService>();
builder.Services.AddScoped<ITodoListQueryService, TodoListQueryService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
