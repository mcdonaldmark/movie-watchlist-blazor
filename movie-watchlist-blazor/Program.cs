using Microsoft.EntityFrameworkCore;
using movie_watchlist_blazor.Data;
using movie_watchlist_blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Database config
// ----------------------
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(databaseUrl))
{
    Console.WriteLine("WARNING: DATABASE_URL not set. Using fallback config.");
    databaseUrl = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(databaseUrl);
});

// ----------------------
// Blazor
// ----------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// App state
builder.Services.AddScoped<AppState>();

builder.Services.AddHttpClient();

// Movie service: TMDB implementation (register as typed client for IMovieService)
builder.Services.AddHttpClient<IMovieService, TmdbMovieService>(c => c.BaseAddress = new Uri("https://api.themoviedb.org/3/"));

// ----------------------
// Render / Railway PORT
// ----------------------
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    serverOptions.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAntiforgery();

app.MapRazorComponents<movie_watchlist_blazor.Components.App>()
    .AddInteractiveServerRenderMode();

// ----------------------
// Auto-migrate database
// ----------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();