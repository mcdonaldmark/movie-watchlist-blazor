using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Review> Reviews { get; set;}
        public DbSet<Movie> Movies {get; set;}

    }
}
