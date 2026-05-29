using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace movie_watchlist_blazor.Models
{
    public class AppUser : IdentityUser
    {
        public string OAuthProvider { get; set; } = string.Empty;
        public string OAuthProviderId { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}