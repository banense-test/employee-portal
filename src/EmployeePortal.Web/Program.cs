using EmployeePortal.Application;
using EmployeePortal.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// CON-029: Razor Pages, no SPA, no client-side router. A page-level script on an
// already-rendered page is Razor Pages as normal, and the clocking page needs one.
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

// Liveness probe. The portal's own use cases are implemented in Elaboration; this
// endpoint exists so the pipeline exercises a real request path from the first build.
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    applicationComponents = ApplicationComponentMap.All.Count,
    infrastructureComponents = InfrastructureComponentMap.All.Count,
}));

app.Run();

// Exposed so the test project can host the application in-process.
public partial class Program;
