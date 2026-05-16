namespace movie_watchlist_blazor.Models;

public class MovieDto
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Genre { get; set; } = "";

    public int Year { get; set; }

    public double AverageRating { get; set; }
}