namespace MovieWatchlist.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Genre { get; set; } = "";
    public bool IsWatched { get; set; }
    public int Rating { get; set; }
    public string Description { get; set; } = "";
}