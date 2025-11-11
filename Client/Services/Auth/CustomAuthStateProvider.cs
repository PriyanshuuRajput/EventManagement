using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EventBooking.Client.Services.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        public readonly IJSRuntime jsRuntime;

        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            var identity = new ClaimsIdentity();
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    var claims = jwtToken.Claims;
                    identity = new ClaimsIdentity(claims, "jwt");
                }
                else
                {
                    await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                }
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticateUser = new ClaimsPrincipal(
                new ClaimsIdentity(new JwtSecurityTokenHandler().ReadJwtToken(token).Claims, "jwt"));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticateUser)));


        }

        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        }
    }
}