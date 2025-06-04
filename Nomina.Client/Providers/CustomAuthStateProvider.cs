using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nomina.Client.Providers;

public class CustomAuthStateProvider(ILocalStorageService localStorage)
    : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var claims = ParseClaimsFromJwt(token);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
    }

    public void NotifyAuthState() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static List<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        var claims = token.Claims.ToList();

        var roleClaim = claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        if (roleClaim != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
        }

        return claims;
    }
}