namespace movie_watchlist_blazor.Services;

public class AppState
{
    public bool IsLoggedIn { get; set; }

    public string CurrentUserName { get; set; } = "";
    
}