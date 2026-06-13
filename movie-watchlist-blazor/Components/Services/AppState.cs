using Microsoft.JSInterop;
using System.Text.Json;
using movie_watchlist_blazor.Models;
using movie_watchlist_blazor.Services;

namespace movie_watchlist_blazor.Services;

public class AppState
{
    private readonly IJSRuntime _js;

    public AppState(IJSRuntime js)
    {
        _js = js;
        // subscribe to MovieState changes to persist movies
        MovieState.OnChange += HandleMovieStateChanged;
    }

    public string CurrentUserName { get; private set; } = "";

    public event Action? OnChange;

    private void NotifyStateChanged()
        => OnChange?.Invoke();

    // Sets the users current username using local storage on the device.
    // Gets the users name or returns and empty set of strings if no name exists
    public async Task SetUserAsync(string name)
    {
        CurrentUserName = name ?? "";

        await _js.InvokeVoidAsync("sessionStorage.setItem", "user", CurrentUserName);

        NotifyStateChanged();
    }

    public async Task LoadUserAsync()
    {
        // First tries to load a users name, but will return an empty set of strings
        // if there is no name that is returned from the users local storage
        try
        {
            var name = await _js.InvokeAsync<string>("sessionStorage.getItem", "user");
            CurrentUserName = string.IsNullOrWhiteSpace(name) ? "" : name;
        }
        catch
        {
            CurrentUserName = "";
        }

        // load persisted movies as well
        await LoadMoviesAsync();

        NotifyStateChanged();
    }

    private async void HandleMovieStateChanged()
    {
        try
        {
            await SaveMoviesAsync();
        }
        catch
        {
            // ignore
        }
    }

    public async Task SaveMoviesAsync()
    {
        // First tries to set the users movie into local storage so that it can be parsed by JSON
        try
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(MovieState.Movies, options);
            await _js.InvokeVoidAsync("sessionStorage.setItem", "movies", json);
        }
        catch
        {
            // swallow
        }
    }

    public async Task LoadMoviesAsync()
    {
        // try/catch block to load the users movie data from their local storage.
        // Gets the movies and loads them into the app so that the user can view them
        try
        {
            var json = await _js.InvokeAsync<string>("sessionStorage.getItem", "movies");
            if (!string.IsNullOrWhiteSpace(json))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var list = JsonSerializer.Deserialize<List<MovieDto>>(json, options);
                if (list != null)
                {
                    MovieState.SetMovies(list);
                }
            }
        }
        catch
        {
            // ignore
        }
    }

    ~AppState()
    {
        MovieState.OnChange -= HandleMovieStateChanged;
    }

    public async Task LogoutAsync()
    {
        // Ensures that the user has successfully logged out of the application
        CurrentUserName = "";

        await _js.InvokeVoidAsync("sessionStorage.removeItem", "user");

        NotifyStateChanged();
    }
}