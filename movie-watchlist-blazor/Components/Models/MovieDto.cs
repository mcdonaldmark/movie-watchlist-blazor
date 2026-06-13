namespace movie_watchlist_blazor.Models;

public class MovieDto
// Model data that is needed for the different movies that are shown to the user
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Genre { get; set; } = "";

    public List<int> GenreIds { get; set; } = new();

    public int Year { get; set; }

    public string ReleaseDate { get; set; } = "";

    public double AverageRating { get; set; }

    public string Overview { get; set; } = "";

    public string PosterUrl { get; set; } = "";

    public string BackdropUrl { get; set; } = "";

    public string TrailerKey { get; set; } = "";
}