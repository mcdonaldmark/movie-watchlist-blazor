using movie_watchlist_blazor.Models;

namespace movie_watchlist_blazor.Services;

public class MockMovieService : IMovieService
{
    // Basic start to a movie list that is included to the user
    private readonly List<MovieDto> _movies = new()
    {
        new MovieDto { Id = 1, Title = "Inception", Genre = "Sci-Fi", GenreIds = new List<int> { 2 }, Year = 2010, AverageRating = 4.8 },
        new MovieDto { Id = 2, Title = "Interstellar", Genre = "Sci-Fi", GenreIds = new List<int> { 2 }, Year = 2014, AverageRating = 4.7 },
        new MovieDto { Id = 3, Title = "The Dark Knight", Genre = "Action", GenreIds = new List<int> { 1 }, Year = 2008, AverageRating = 4.9 }
    };

    private readonly Dictionary<int, List<ReviewDto>> _reviews = new();
    private readonly List<int> _watchlist = new();

    private int _reviewIdCounter = 1;

    // ================= MOVIES =================

    // Gets the info on the movies and adds them to a list so that it can be looped through and shown to the user
    public Task<List<MovieDto>> GetMoviesAsync(int page = 1, int? genreId = null)
    {
        var movies = _movies.AsEnumerable();

        if (genreId.HasValue)
        {
            movies = movies.Where(m => m.GenreIds.Contains(genreId.Value));
        }

        return Task.FromResult(movies.Skip((page - 1) * 20).Take(20).ToList());
    }

    // Has a list of different genres that the user can choose from when looking for a movei to watch
    // Each genre has it's own ID
    public Task<List<GenreDto>> GetGenresAsync()
    {
        var genres = new List<GenreDto>
        {
            new GenreDto { Id = 1, Name = "Action" },
            new GenreDto { Id = 2, Name = "Sci-Fi" },
            new GenreDto { Id = 3, Name = "Drama" }
        };

        return Task.FromResult(genres);
    }

    // Gets the movie by their ID and returns it
    public Task<MovieDto?> GetMovieByIdAsync(int movieId)
    {
        var m = _movies.FirstOrDefault(mv => mv.Id == movieId);
        return Task.FromResult<MovieDto?>(m);
    }

    // Adds the movie to a watched list so that the user can see what they've watched
    // Checks to see if the watchlist already has the movie ID in it
    public Task AddToWatchlistAsync(int movieId)
    {
        if (!_watchlist.Contains(movieId))
            _watchlist.Add(movieId);

        return Task.CompletedTask;
    }

    // ================= REVIEWS =================

    // Function to get the reviews that the different users have set
    public Task<List<ReviewDto>> GetReviewsAsync(int movieId)
    {
        _reviews.TryGetValue(movieId, out var list);
        return Task.FromResult(list ?? new List<ReviewDto>());
    }

    // Function to add a review to the movies that are available to the user
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

    // Function to update a review that a user wants to leave on a movie
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

    // Function to delete a movie review that the user specifies
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

    // A function that will update the average rating of the movie once a review is left by the user
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