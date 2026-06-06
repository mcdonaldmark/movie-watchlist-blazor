using Microsoft.JSInterop;

namespace movie_watchlist_blazor.Services;

public class AppState
{
    private readonly IJSRuntime _js;

    public AppState(IJSRuntime js)
    {
        _js = js;
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

        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        CurrentUserName = "";

        await _js.InvokeVoidAsync("sessionStorage.removeItem", "user");

        NotifyStateChanged();
    }
}