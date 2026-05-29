using System.ComponentModel.DataAnnotations;

namespace movie_watchlist_blazor.Models;

public class Movie
{
    [Key]
    public int MovieId { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public int Year { get; set; }

    public string? PosterUrl { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}