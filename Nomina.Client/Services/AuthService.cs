using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Nomina.Client.Providers;
using System.Net.Http.Json;

namespace Nomina.Client.Services;

public class AuthService(
    HttpClient http,
    AuthenticationStateProvider authProvider,
    ILocalStorageService localStorage,
    NavigationManager nav)
{
    public async Task<HttpResponseMessage> Login(string username, string password)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", new { username, password });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            await localStorage.SetItemAsync("authToken", result?.Token ?? string.Empty);
            ((CustomAuthStateProvider)authProvider).NotifyAuthState();
        }

        return response;
    }

    public async Task Logout()
    {
        await localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)authProvider).NotifyAuthState();
        nav.NavigateTo("/login", forceLoad: true);
    }
}

public record LoginResult(string Token);