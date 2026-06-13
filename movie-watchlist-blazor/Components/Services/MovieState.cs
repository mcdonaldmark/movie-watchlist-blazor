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

    // Toggles in the users dashboard if the movie has been watched
    public static void ToggleWatched(int movieId)
    {
        if (WatchedMovieIds.Contains(movieId))
            WatchedMovieIds.Remove(movieId);
        else
            WatchedMovieIds.Add(movieId);
        NotifyStateChanged();
    }

    // Moves the movie ID into a list once the user specifies that they've watched the movie
    public static bool IsWatched(int movieId)
        => WatchedMovieIds.Contains(movieId);

    // Adds a movie to the users list
    public static void AddMovie(MovieDto movie)
    {
        if (!Movies.Any(m => m.Id == movie.Id))
        {
            Movies.Add(movie);
            NotifyStateChanged();
        }
    }

    // Sets the movies into a list for the user
    public static void SetMovies(List<MovieDto> movies)
    {
        Movies = movies ?? new List<MovieDto>();
        NotifyStateChanged();
    }

    // Removes a movie from the users dashboard
    public static void RemoveMovie(int movieId)
    {
        Movies.RemoveAll(m => m.Id == movieId);
        WatchedMovieIds.Remove(movieId);
        NotifyStateChanged();
    }

    // FUTURE: these will be replaced by API calls
}