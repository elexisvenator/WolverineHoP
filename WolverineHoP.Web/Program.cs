using WolverineHoP.ServiceDefaults;
using WolverineHoP.Web.Api.Vanilla;
using WolverineHoP.Web.Api.WolverineDocument;
using WolverineHoP.Web.Api.WolverineEvents;
using WolverineHoP.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<VanillaApiClient>(client =>
{
    // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
    // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
    client.BaseAddress = new("https+http://vanilla-api");
});
builder.Services.AddHttpClient<WolverineDocumentApiClient>(client =>
{
    client.BaseAddress = new("https://localhost:7082");
});
builder.Services.AddHttpClient<WolverineEventsApiClient>(client =>
{
    client.BaseAddress = new("https://localhost:7003");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
