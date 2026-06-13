using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using movie_watchlist_blazor.Models;

// Displays movies to the user from a third-party API system so that the user can have a wide
// range of different movies that they can choose from

namespace movie_watchlist_blazor.Services;

public class TmdbMovieService : IMovieService
{
    // Temporary in memory data for the movie info which will be sent to the database later
    private readonly HttpClient _http;
    private readonly string _apiKey;

    private readonly Dictionary<int, List<ReviewDto>> _reviews = new();
    private readonly List<int> _watchlist = new();
    private int _reviewIdCounter = 1;

    private readonly Dictionary<int, string> _genreMap = new();

    public TmdbMovieService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["TMDB:ApiKey"] ?? string.Empty;
    }

    // Pulls the genres of the different movies from the API and so that they can be
    // displayed to the user. Maps out both the id and the name of the movie for the user
    private async Task EnsureGenresAsync()
    {
        if (_genreMap.Any()) return;

        var res = await _http.GetAsync($"genre/movie/list?api_key={_apiKey}&language=en-US");
        if (!res.IsSuccessStatusCode) return;

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        if (doc.RootElement.TryGetProperty("genres", out var arr))
        {
            foreach (var g in arr.EnumerateArray())
            {
                var id = g.GetProperty("id").GetInt32();
                var name = g.GetProperty("name").GetString() ?? string.Empty;
                _genreMap[id] = name;
            }
        }
    }

    public async Task<List<GenreDto>> GetGenresAsync()
    {
        await EnsureGenresAsync();
        return _genreMap.Select(g => new GenreDto { Id = g.Key, Name = g.Value }).OrderBy(g => g.Name).ToList();
    }

    // Pulls the movies from the API and parses them so that they can be displayed to the user
    // Displays info on each movie as it loops through the entire list of movies by their genre
    public async Task<List<MovieDto>> GetMoviesAsync(int page = 1, int? genreId = null)
    {
        await EnsureGenresAsync();

        string url = genreId.HasValue
            ? $"discover/movie?api_key={_apiKey}&language=en-US&sort_by=popularity.desc&page={page}&with_genres={genreId.Value}"
            : $"movie/popular?api_key={_apiKey}&language=en-US&page={page}";

        var res = await _http.GetAsync(url);
        res.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());

        var list = new List<MovieDto>();
        if (doc.RootElement.TryGetProperty("results", out var results))
        {
            foreach (var r in results.EnumerateArray())
            {
                var id = r.GetProperty("id").GetInt32();
                var title = r.GetProperty("title").GetString() ?? string.Empty;
                var overview = r.GetProperty("overview").GetString() ?? string.Empty;
                var poster = r.GetProperty("poster_path").GetString();
                var backdrop = r.GetProperty("backdrop_path").GetString();
                var vote = r.GetProperty("vote_average").GetDouble();
                var release = r.TryGetProperty("release_date", out var rd) ? rd.GetString() : string.Empty;
                var year = 0;
                if (!string.IsNullOrEmpty(release) && release.Length >= 4)
                    int.TryParse(release.Substring(0, 4), out year);

                var genreName = string.Empty;
                var genreIds = new List<int>();
                if (r.TryGetProperty("genre_ids", out var gids))
                {
                    foreach (var gid in gids.EnumerateArray())
                    {
                        var gidv = gid.GetInt32();
                        genreIds.Add(gidv);
                        if (string.IsNullOrEmpty(genreName) && _genreMap.TryGetValue(gidv, out var gname))
                        {
                            genreName = gname;
                        }
                    }
                }

                list.Add(new MovieDto
                {
                    Id = id,
                    Title = title,
                    Overview = overview,
                    PosterUrl = string.IsNullOrEmpty(poster) ? string.Empty : $"https://image.tmdb.org/t/p/w500{poster}",
                    BackdropUrl = string.IsNullOrEmpty(backdrop) ? string.Empty : $"https://image.tmdb.org/t/p/w780{backdrop}",
                    AverageRating = vote,
                    Year = year,
                    ReleaseDate = release ?? string.Empty,
                    Genre = genreName,
                    GenreIds = genreIds
                });
            }
        }

        return list;
    }

    // Gets relevant info about the movies to display to the user such as title, overview, release_date, etc
    // So that the user has more info about the movies they can add to their watchlist
    public async Task<MovieDto?> GetMovieByIdAsync(int movieId)
    {
        await EnsureGenresAsync();

        var res = await _http.GetAsync($"movie/{movieId}?api_key={_apiKey}&language=en-US&append_to_response=videos");
        if (!res.IsSuccessStatusCode) return null;

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        var title = root.GetProperty("title").GetString() ?? string.Empty;
        var overview = root.GetProperty("overview").GetString() ?? string.Empty;
        var poster = root.GetProperty("poster_path").GetString();
        var backdrop = root.GetProperty("backdrop_path").GetString();
        var vote = root.GetProperty("vote_average").GetDouble();
        var release = root.TryGetProperty("release_date", out var rd) ? rd.GetString() : null;
        var year = 0;
        if (!string.IsNullOrEmpty(release) && release.Length >= 4)
            int.TryParse(release.Substring(0, 4), out year);

        var genreName = string.Empty;
        var genreIds = new List<int>();
        if (root.TryGetProperty("genres", out var genres))
        {
            foreach (var g in genres.EnumerateArray())
            {
                var id = g.GetProperty("id").GetInt32();
                genreIds.Add(id);
                if (string.IsNullOrEmpty(genreName))
                {
                    genreName = g.GetProperty("name").GetString() ?? string.Empty;
                }
            }
        }

        var trailerKey = string.Empty;
        if (root.TryGetProperty("videos", out var videos) && videos.TryGetProperty("results", out var vids))
        {
            foreach (var v in vids.EnumerateArray())
            {
                var site = v.GetProperty("site").GetString();
                var type = v.GetProperty("type").GetString();
                if (string.Equals(site, "YouTube", StringComparison.OrdinalIgnoreCase) && string.Equals(type, "Trailer", StringComparison.OrdinalIgnoreCase))
                {
                    trailerKey = v.GetProperty("key").GetString() ?? string.Empty;
                    break;
                }
            }
        }

        // Returns a movie to display to the user with all of the relevant info so that the user
        // can view details about the movie they want to add to their watchlist
        return new MovieDto
        {
            Id = movieId,
            Title = title,
            Overview = overview,
            PosterUrl = string.IsNullOrEmpty(poster) ? string.Empty : $"https://image.tmdb.org/t/p/w500{poster}",
            BackdropUrl = string.IsNullOrEmpty(backdrop) ? string.Empty : $"https://image.tmdb.org/t/p/w780{backdrop}",
            AverageRating = vote,
            Year = year,
            ReleaseDate = release ?? string.Empty,
            Genre = genreName,
            GenreIds = genreIds,
            TrailerKey = trailerKey
        };
    }

    // ================= REVIEWS & WATCHLIST (in-memory, same behavior as MockMovieService)

    public Task AddToWatchlistAsync(int movieId)
    {
        if (!_watchlist.Contains(movieId))
            _watchlist.Add(movieId);
        return Task.CompletedTask;
    }

    public Task<List<ReviewDto>> GetReviewsAsync(int movieId)
    {
        _reviews.TryGetValue(movieId, out var list);
        return Task.FromResult(list ?? new List<ReviewDto>());
    }

    // Function to add a review to a movie so that the review can be seen by others
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

    // Function to update a review that a user wants to edit and save so that others can see
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

    // Function to delete a review from a movie that a user selects
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

    // Updates the average rating for the movie that is shown to the user
    private void UpdateAverageRating(int movieId)
    {
        if (!_reviews.TryGetValue(movieId, out var list) || list.Count == 0)
            return;

        var avg = list.Average(r => r.Rating);
        // No local movie list to update average on; reviews are stored separately.
    }
}
