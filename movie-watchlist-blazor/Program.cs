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
