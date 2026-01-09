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

            //  auth handler(adds Jwt)
            builder.Services.AddTransient<AuthMessageHandler>();

            //  API HttpClient (Production)
            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri("http://priyanshuu007-001-site1.ktempurl.com/");
            });

            //  AUTHORIZED API (JWT attached)
            builder.Services.AddHttpClient("AuthorizedApi", client =>
            {
                client.BaseAddress = new Uri("http://priyanshuu007-001-site1.ktempurl.com/");
            })
            .AddHttpMessageHandler<AuthMessageHandler>();

            //  DEFAULT HttpClient = PUBLIC API
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));


            // Services
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<ClientEventService>();
            builder.Services.AddScoped<SpinnerService>();
            builder.Services.AddScoped<ManagerAccountService>();


            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredSessionStorage();
            //builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<LoginOverlayService>();



            // Auth state provider
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
                provider.GetRequiredService<CustomAuthStateProvider>());

            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
