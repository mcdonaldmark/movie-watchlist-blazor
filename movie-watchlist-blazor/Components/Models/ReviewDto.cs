namespace movie_watchlist_blazor.Models;

public class ReviewDto
{
    // Database model that is needed for a user trying to save a review on a movie that they like
    public int Id { get; set; }
    public int MovieId { get; set; }

    public string UserName { get; set; } = "User";
    public string Comment { get; set; } = "";
    public int Rating { get; set; }
}