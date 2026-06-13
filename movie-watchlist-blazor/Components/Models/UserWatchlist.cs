public class UserWatchlist
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int MovieId { get; set; }

    public bool IsWatched { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
