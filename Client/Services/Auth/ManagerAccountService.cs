using Applications.Dto.OrganizerDto;
using System.Net.Http.Json;

namespace EventBooking.Client.Services.Auth
{
    public class ManagerAccountService
    {
        private readonly HttpClient httpClient;
        public ManagerAccountService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SignupManagerAsync(ManagerSignUpDto dto)
        {
            return await httpClient.PostAsJsonAsync("api/Account/manager-signup", dto);
        }
    }
}
