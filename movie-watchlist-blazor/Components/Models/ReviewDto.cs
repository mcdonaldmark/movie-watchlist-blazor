namespace movie_watchlist_blazor.Models;

public class ReviewDto
{
    public int Id { get; set; }
    public int MovieId { get; set; }

    public string UserName { get; set; } = "User";
    public string Comment { get; set; } = "";
    public int Rating { get; set; }
}