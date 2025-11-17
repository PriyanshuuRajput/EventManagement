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

            // Register auth handler
            builder.Services.AddTransient<AuthMessageHandler>();

            // Authorized HttpClient: attaches JWT token automatically
            builder.Services.AddHttpClient("AuthorizedAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7239/");
            })
            .AddHttpMessageHandler<AuthMessageHandler>();

            // Set THIS as the default HttpClient everywhere
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedAPI"));

            // Services
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<ClientEventService>();
            builder.Services.AddScoped<SpinnerService>();

            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredSessionStorage();

            // Auth state provider
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
                provider.GetRequiredService<CustomAuthStateProvider>());

            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
