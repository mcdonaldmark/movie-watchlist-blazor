using Microsoft.EntityFrameworkCore;
using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
    }
}