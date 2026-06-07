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

    public async Task SetUserAsync(string name)
    {
        CurrentUserName = name ?? "";

        await _js.InvokeVoidAsync("sessionStorage.setItem", "user", CurrentUserName);

        NotifyStateChanged();
    }

    public async Task LoadUserAsync()
    {
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
        CurrentUserName = "";

        await _js.InvokeVoidAsync("sessionStorage.removeItem", "user");

        NotifyStateChanged();
    }
}