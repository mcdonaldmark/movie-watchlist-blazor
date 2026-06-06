using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using movie_watchlist_blazor;
using movie_watchlist_blazor.Data;
using movie_watchlist_blazor.Models;
using movie_watchlist_blazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .Services.AddScoped<AppState>();
        
builder.WebHost.ConfigureKestrel(serverOptions =>

{

    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

    serverOptions.ListenAnyIP(int.Parse(port));

});
 


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    if (!db.Movies.Any())
    {
        db.Movies.AddRange(
            new Movie { Title = "Inception", Genre = "Sci-Fi", Year = 2010, PosterUrl = "https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg", CreateDate = DateTime.UtcNow },
            new Movie { Title = "The Dark Knight", Genre = "Action", Year = 2008, PosterUrl = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg", CreateDate = DateTime.UtcNow },
            new Movie { Title = "Interstellar", Genre = "Sci-Fi", Year = 2014, PosterUrl = "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIe.jpg", CreateDate = DateTime.UtcNow },
            new Movie { Title = "Parasite", Genre = "Thriller", Year = 2019, PosterUrl = "https://image.tmdb.org/t/p/w500/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg", CreateDate = DateTime.UtcNow },
            new Movie { Title = "Your Name", Genre = "Animation", Year = 2016, PosterUrl = "https://image.tmdb.org/t/p/w500/q719jXXEzOoYaps6babgKnONONX.jpg", CreateDate = DateTime.UtcNow }
        );
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<movie_watchlist_blazor.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
