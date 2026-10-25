using ContactAgenda.Components;
using ContactAgenda.Application;
using ContactAgenda.Endpoints;
using ContactAgenda.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Contacts") ?? "Data Source=contacts.db"));
builder.Services.AddScoped<IContactRepository, EfContactRepository>();
builder.Services.AddScoped<ContactService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapContactEndpoints();

app.Run();

public partial class Program
{
}
