using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Services;

public static class MovieState
{
    // TEMPORARY PLACEHOLDERS ONLY (until API exists)
    public static List<MovieDto> Movies { get; set; } = new();

    public static HashSet<int> WatchedMovieIds { get; set; } = new();

    public static void ToggleWatched(int movieId)
    {
        if (WatchedMovieIds.Contains(movieId))
            WatchedMovieIds.Remove(movieId);
        else
            WatchedMovieIds.Add(movieId);
    }

    public static bool IsWatched(int movieId)
        => WatchedMovieIds.Contains(movieId);

    // FUTURE: these will be replaced by API calls
}