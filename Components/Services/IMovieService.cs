using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Services;

public interface IMovieService
{
    Task<List<MovieDto>> GetMoviesAsync();

    Task<List<ReviewDto>> GetReviewsAsync(int movieId);

    Task AddToWatchlistAsync(int movieId);

    Task AddReviewAsync(int movieId, ReviewDto review);

    Task UpdateReviewAsync(int movieId, ReviewDto review);
    Task DeleteReviewAsync(int movieId, int reviewId);
}