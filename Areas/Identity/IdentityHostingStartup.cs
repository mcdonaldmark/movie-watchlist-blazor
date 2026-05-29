using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(movie_watchlist_blazor.Areas.Identity.IdentityHostingStartup))]

namespace movie_watchlist_blazor.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
        }
    }
}
