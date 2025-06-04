using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace Nomina.Client.Handlers;

public class AuthTokenHandler(ILocalStorageService _localStorage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Obtiene el token del localStorage (con token de cancelación)
        var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            // Agrega el token al header Authorization
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}