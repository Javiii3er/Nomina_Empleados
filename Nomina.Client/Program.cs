using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Nomina.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Nomina.Client.Providers;
using Nomina.Client.Services;
using Blazored.LocalStorage;
using System.Security.Claims;

namespace Nomina.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Configuración básica
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        });

        // Configuración de autenticación
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim(ClaimTypes.Role, "Admin"));
            options.AddPolicy("HR_Plus", policy =>
                policy.RequireClaim(ClaimTypes.Role, ["Admin", "HR"]));
        });
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<AuthenticatedHttpClient>();

        var host = builder.Build();

        var authenticatedHttpClient = host.Services.GetRequiredService<AuthenticatedHttpClient>();
        await authenticatedHttpClient.InitializeAsync();

        await host.RunAsync();
    }
}

#pragma warning disable IDE0290
public sealed class AuthenticatedHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AuthenticatedHttpClient(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public HttpClient Client => _httpClient;

    public async Task InitializeAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
        }
    }

    public void Dispose() => _httpClient?.Dispose();
}
#pragma warning restore IDE0290