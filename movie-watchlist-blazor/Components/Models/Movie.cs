namespace movie_watchlist_blazor.Models;

public class Movie
{
    public string Title { get; set; } = "";
    public string Genre { get; set; } = "";
    public string Description { get; set; } = "";

    public int Rating { get; set; }
    public bool IsWatched { get; set; }
}