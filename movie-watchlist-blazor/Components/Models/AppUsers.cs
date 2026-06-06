    using System.ComponentModel.DataAnnotations;

    namespace movie_watchlist_blazor.Models
    {
        public class AppUser
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; } = "";

            [Required]
            public string Email { get; set; } = "";

            [Required]
            public string Password { get; set; } = "";

            public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        }
    }