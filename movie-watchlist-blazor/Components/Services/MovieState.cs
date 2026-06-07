using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Services;

public static class MovieState
{
    // TEMPORARY PLACEHOLDERS ONLY (until API exists)
    public static List<MovieDto> Movies { get; set; } = new();

    public static HashSet<int> WatchedMovieIds { get; set; } = new();

    // Event to notify UI components of changes
    public static event Action? OnChange;

    private static void NotifyStateChanged() => OnChange?.Invoke();

    public static void ToggleWatched(int movieId)
    {
        if (WatchedMovieIds.Contains(movieId))
            WatchedMovieIds.Remove(movieId);
        else
            WatchedMovieIds.Add(movieId);
        NotifyStateChanged();
    }

    public static bool IsWatched(int movieId)
        => WatchedMovieIds.Contains(movieId);

    public static void AddMovie(MovieDto movie)
    {
        if (!Movies.Any(m => m.Id == movie.Id))
        {
            Movies.Add(movie);
            NotifyStateChanged();
        }
    }

    public static void SetMovies(List<MovieDto> movies)
    {
        Movies = movies ?? new List<MovieDto>();
        NotifyStateChanged();
    }

    public static void RemoveMovie(int movieId)
    {
        Movies.RemoveAll(m => m.Id == movieId);
        WatchedMovieIds.Remove(movieId);
        NotifyStateChanged();
    }

    // FUTURE: these will be replaced by API calls
}