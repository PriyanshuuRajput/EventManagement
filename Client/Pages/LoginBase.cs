using Applications.Dto.AccountDto;
using EventBooking.Client.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

public class LoginBase : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected CustomAuthStateProvider AuthStateProvider { get; set; } = default!;

    protected LoginDto loginModel = new();
    protected string errorMessage = "";

    protected async Task<bool> LoginAsync(string url, string redirectUrl)
    {
        errorMessage = "";

        try
        {
            var response = await Http.PostAsJsonAsync(url, loginModel);

            if (!response.IsSuccessStatusCode)
            {
                errorMessage = await response.Content.ReadAsStringAsync();
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                errorMessage = "Invalid login response.";
                return false;
            }

            // Save JWT, role, and userId
            await JS.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
            await JS.InvokeVoidAsync("localStorage.setItem", "userRole", result.Role ?? "");
            await JS.InvokeVoidAsync("localStorage.setItem", "userId", result.UserId.ToString());

            // Notify state provider
            AuthStateProvider.NotifyUserAuthentication(result.Token);

            // Redirect
            Navigation.NavigateTo(redirectUrl, true);

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = "Login failed: " + ex.Message;
            return false;
        }
    }

    public class LoginResponse
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
        public int UserId { get; set; }
    }
}
