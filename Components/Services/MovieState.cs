using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Services;

public static class MovieState
{
    public static List<MovieDto> Movies { get; set; } = new()
    {
        new MovieDto { Id = 1, Title = "Inception", Genre = "Sci-Fi", Year = 2010, AverageRating = 4.8 },
        new MovieDto { Id = 2, Title = "Interstellar", Genre = "Sci-Fi", Year = 2014, AverageRating = 4.7 },
        new MovieDto { Id = 3, Title = "The Dark Knight", Genre = "Action", Year = 2008, AverageRating = 4.9 }
    };

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
}