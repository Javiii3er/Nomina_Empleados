using Microsoft.AspNetCore.Components;
using System.Net;

namespace Nomina.Client.Handlers;

public class ApiErrorHandler(NavigationManager _nav) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _nav.NavigateTo("/logout?redirect=unauthorized");
        }

        return response;
    }
}