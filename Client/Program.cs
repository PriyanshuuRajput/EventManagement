using Blazored.SessionStorage;
using Blazored.Toast;
using EventBooking.Client;
using EventBooking.Client.Services;
using EventBooking.Client.Services.Auth;
using EventBooking.Client.SpinnerService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Base HttpClient for public requests (no auth)
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7239")
            });

            // Register custom Auth handler to attach token
            builder.Services.AddScoped<AuthMessageHandler>();

            // HttpClient for AdminService (includes JWT automatically)
            builder.Services.AddHttpClient<AdminService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7239/");
            }).AddHttpMessageHandler<AuthMessageHandler>();

            // Register other services

            builder.Services.AddScoped<ClientEventService>();
            builder.Services.AddScoped<SpinnerService>();
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddBlazoredToast();



            // Add authentication state provider
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());



            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredSessionStorage();

            await builder.Build().RunAsync();
        }
    }
}
