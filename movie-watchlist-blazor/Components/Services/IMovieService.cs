using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Services;

public interface IMovieService
{
    Task<List<MovieDto>> GetMoviesAsync(int page = 1, int? genreId = null);

    Task<List<GenreDto>> GetGenresAsync();

    Task<MovieDto?> GetMovieByIdAsync(int movieId);

    Task<List<ReviewDto>> GetReviewsAsync(int movieId);

    Task AddToWatchlistAsync(int movieId);

    Task AddReviewAsync(int movieId, ReviewDto review);

    Task UpdateReviewAsync(int movieId, ReviewDto review);
    Task DeleteReviewAsync(int movieId, int reviewId);
}