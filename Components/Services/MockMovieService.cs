using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Services;

public class MockMovieService : IMovieService
{
    private readonly List<MovieDto> _movies = new()
    {
        new MovieDto { Id = 1, Title = "Inception", Genre = "Sci-Fi", Year = 2010, AverageRating = 4.8 },
        new MovieDto { Id = 2, Title = "Interstellar", Genre = "Sci-Fi", Year = 2014, AverageRating = 4.7 },
        new MovieDto { Id = 3, Title = "The Dark Knight", Genre = "Action", Year = 2008, AverageRating = 4.9 }
    };

    private readonly Dictionary<int, List<ReviewDto>> _reviews = new();
    private readonly List<int> _watchlist = new();

    private int _reviewIdCounter = 1;

    // ================= MOVIES =================

    public Task<List<MovieDto>> GetMoviesAsync()
    {
        return Task.FromResult(_movies);
    }

    public Task AddToWatchlistAsync(int movieId)
    {
        if (!_watchlist.Contains(movieId))
            _watchlist.Add(movieId);

        return Task.CompletedTask;
    }

    // ================= REVIEWS =================

    public Task<List<ReviewDto>> GetReviewsAsync(int movieId)
    {
        _reviews.TryGetValue(movieId, out var list);
        return Task.FromResult(list ?? new List<ReviewDto>());
    }

    public Task AddReviewAsync(int movieId, ReviewDto review)
    {
        if (!_reviews.ContainsKey(movieId))
            _reviews[movieId] = new List<ReviewDto>();

        review.Id = _reviewIdCounter++;
        review.MovieId = movieId;

        _reviews[movieId].Add(review);

        UpdateAverageRating(movieId);

        return Task.CompletedTask;
    }

    public Task UpdateReviewAsync(int movieId, ReviewDto review)
    {
        if (_reviews.TryGetValue(movieId, out var list))
        {
            var existing = list.FirstOrDefault(r => r.Id == review.Id);

            if (existing != null)
            {
                existing.Comment = review.Comment;
                existing.Rating = review.Rating;

                UpdateAverageRating(movieId);
            }
        }

        return Task.CompletedTask;
    }

    public Task DeleteReviewAsync(int movieId, int reviewId)
    {
        if (_reviews.TryGetValue(movieId, out var list))
        {
            var review = list.FirstOrDefault(r => r.Id == reviewId);

            if (review != null)
            {
                list.Remove(review);
                UpdateAverageRating(movieId);
            }
        }

        return Task.CompletedTask;
    }

    // ================= HELPERS =================

    private void UpdateAverageRating(int movieId)
    {
        if (!_reviews.TryGetValue(movieId, out var list) || list.Count == 0)
        {
            var movie = _movies.FirstOrDefault(m => m.Id == movieId);
            if (movie != null) movie.AverageRating = 0;
            return;
        }

        var avg = list.Average(r => r.Rating);

        var targetMovie = _movies.FirstOrDefault(m => m.Id == movieId);
        if (targetMovie != null)
            targetMovie.AverageRating = avg;
    }
}