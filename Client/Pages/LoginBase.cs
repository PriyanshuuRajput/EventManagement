using Applications.Dto.AccountDto;
using EventBooking.Client.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;


namespace EventBooking.Client.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected CustomAuthStateProvider AuthStateProvider { get; set; } = default!;


        protected LoginDto loginModel = new();
        protected string errorMessage = "";

        protected async Task<bool> LoginAsync(string apiUrl, string redirectUrl)
        {

            errorMessage = "";
            try
            {
                var response = await Http.PostAsJsonAsync(apiUrl, loginModel);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await JS.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
                        await JS.InvokeVoidAsync("localStorage.setItem", "userRole", result.Role ?? "");


                        // Notify authentication
                        AuthStateProvider.NotifyUserAuthentication(result.Token);

                        // Redirect
                        Nav.NavigateTo(redirectUrl, true);
                        return true;
                    }
                    else
                    {
                        errorMessage = "Invalid credentials.";
                    }
                }
                else
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    errorMessage = $"Login failed:{resp}";

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

            }
            return false;

        }
        protected class LoginResponse
        {
            public string? Token { get; set; }
            public string? Role { get; set; }
        }

    }
}
