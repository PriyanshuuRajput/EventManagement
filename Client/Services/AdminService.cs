using Applications.Dto;
using Applications.Dto.OrganizerDto;
using Applications.Dto.Pagination;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EventBooking.Client.Services
{
    public class AdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public AdminService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        // 🔐 Add bearer token from localStorage
        private async Task AddAuthHeaderAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            _httpClient.DefaultRequestHeaders.Authorization =
                !string.IsNullOrEmpty(token) ? new AuthenticationHeaderValue("Bearer", token) : null;
        }


        //------------------Managers----------------------//

        public async Task<List<ManagerProfileDto>> GetAllManagersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ManagerProfileDto>>("api/Admin/managers") ?? new();
        }

        // -------------------- EVENTS -------------------- //
        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            await AddAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<List<EventDto>>("api/events") ?? new();
        }

        //public async Task AddEventAsync(EventDto dto, IBrowserFile? file)
        //{
        //    await AddAuthHeaderAsync();

        //    using var content = new MultipartFormDataContent();

        //    // Add Event fields as JSON string
        //    content.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(dto)), "Event");

        //    // Add file if exists
        //    if (file != null)
        //    {
        //        var streamContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10_485_760)); // 10 MB
        //        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        //        content.Add(streamContent, "file", file.Name);
        //    }

        //    var response = await _httpClient.PostAsync("api/events", content);
        //    response.EnsureSuccessStatusCode();
        //}

        public async Task AddEventAsync(EventDto dto, IBrowserFile? file)
        {
            await AddAuthHeaderAsync();

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Title ?? ""), "Title");
            content.Add(new StringContent(dto.EventType ?? ""), "EventType");
            content.Add(new StringContent(dto.Description ?? ""), "Description");
            content.Add(new StringContent(dto.Genre ?? ""), "Genre");
            content.Add(new StringContent(dto.Language ?? ""), "Language");

            // Duration must be in correct TimeSpan format ("hh:mm:ss")
            content.Add(new StringContent(dto.Duration?.ToString(@"hh\:mm\:ss")??""), "Duration");

            content.Add(new StringContent(dto.StartDateOnly.ToString("yyyy-MM-dd")), "StartDateOnly");
            content.Add(new StringContent(dto.StartTime.ToString(@"hh\:mm")), "StartTime");

            content.Add(new StringContent(dto.EndDateOnly?.ToString("yyyy-MM-dd") ?? ""), "EndDateOnly");
            content.Add(new StringContent(dto.EndTime.ToString(@"hh\:mm")), "EndTime");

            //  VenueId and CityId are integers
            content.Add(new StringContent(dto.VenueId.ToString()), "VenueId");
            if (dto.CityId.HasValue)
                content.Add(new StringContent(dto.CityId.Value.ToString()), "CityId");

            content.Add(new StringContent(dto.VenueName ?? ""), "VenueName");
            content.Add(new StringContent(dto.CityName ?? ""), "CityName");
            content.Add(new StringContent(dto.ManagerId?.ToString() ?? "0"), "ManagerId");
            content.Add(new StringContent(dto.ManagerName ?? ""), "ManagerName");

            // ✅Use invariant culture for price
            content.Add(new StringContent(dto.TicketPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)), "TicketPrice");

            if (file != null)
            {
                var stream = new StreamContent(file.OpenReadStream(maxAllowedSize: 10_485_760));
                stream.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(stream, "ImageFile", file.Name);
            }

            var response = await _httpClient.PostAsync("api/events/create", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create event: {response.StatusCode} - {error}");
            }
        }


        public async Task UpdateEventAsync(int id, EventDto dto, IBrowserFile? file)
        {
            await AddAuthHeaderAsync();

            using var content = new MultipartFormDataContent();

            //// 🔹 Match property names exactly with EventDto
            //content.Add(new StringContent(dto.Id.ToString()), "Id");
            content.Add(new StringContent(dto.Title ?? ""), "Title");
            content.Add(new StringContent(dto.EventType ?? ""), "EventType");
            content.Add(new StringContent(dto.Description ?? ""), "Description");
            content.Add(new StringContent(dto.Genre ?? ""), "Genre");
            content.Add(new StringContent(dto.Language ?? ""), "Language");
            content.Add(new StringContent(dto.Duration?.ToString(@"hh\:mm\:ss")??""), "Duration");
            content.Add(new StringContent(dto.StartDateOnly.ToString("yyyy-MM-dd")), "StartDateOnly");
            content.Add(new StringContent(dto.StartTime.ToString(@"hh\:mm")), "StartTime");

            content.Add(new StringContent(dto.EndDateOnly?.ToString("yyyy-MM-dd") ?? ""), "EndDateOnly");
            content.Add(new StringContent(dto.EndTime.ToString(@"hh\:mm")), "EndTime");

            content.Add(new StringContent(dto.VenueId.ToString()), "VenueId");

            if (dto.CityId.HasValue)
                content.Add(new StringContent(dto.CityId.Value.ToString()), "CityId");

            content.Add(new StringContent(dto.VenueName ?? ""), "VenueName");
            content.Add(new StringContent(dto.CityName ?? ""), "CityName");
            content.Add(new StringContent(dto.ManagerId?.ToString() ?? "0"), "ManagerId");
            content.Add(new StringContent(dto.ManagerName ?? ""), "ManagerName");

            content.Add(new StringContent(dto.TicketPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)), "TicketPrice");

            // ✅ Handle image file (if provided)
            if (file != null)
            {
                var stream = new StreamContent(file.OpenReadStream(maxAllowedSize: 10_485_760)); // 10 MB limit
                stream.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(stream, "ImageFile", file.Name);
            }

            var response = await _httpClient.PutAsync($"api/events/update/{id}", content);


            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"UpdateEvent failed: {response.StatusCode} - {error}");
                throw new Exception($"UpdateEvent failed: {response.StatusCode} - {error}");
            }
        }



        public async Task DeleteEventAsync(int id)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/events/{id}");
            response.EnsureSuccessStatusCode();
        }

        // -------------------- VENUES -------------------- //
        public async Task<List<VenueDto>> GetAllVenuesAsync()
        {
            await AddAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<List<VenueDto>>("api/venues") ?? new();
        }

        public async Task AddVenueAsync(VenueDto dto)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/venues", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Server Error: {error}");
            }

        }

        public async Task UpdateVenueAsync(int id, VenueDto dto)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/venues/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVenueAsync(int id)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/venues/{id}");
            response.EnsureSuccessStatusCode();
        }

        // -------------------- CITIES -------------------- //
        public async Task<List<CityDto>> GetAllCitiesAsync()
        {
            await AddAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<List<CityDto>>("api/cities") ?? new();
        }

        public async Task AddCityAsync(CityDto dto)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/cities", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateCityAsync(int id, CityDto dto)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/cities/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCityAsync(int id)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/cities/{id}");
            response.EnsureSuccessStatusCode();
        }

        // -------------------- SEATS -------------------- //
        public async Task<List<SeatDto>> GetAllSeatsAsync()
        {
            await AddAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<List<SeatDto>>("api/seats") ?? new();
        }

        public async Task AddSeatAsync(SeatDto dto)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/seats", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateSeatAsync(int id, SeatDto dto)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/seats/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteSeatAsync(int id)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/seats/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<CountryDto>> GetAllCountriesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CountryDto>>("api/country") ?? new();
        }

        public async Task<List<StateDto>> GetStatesByCountryAsync(Guid countryId)
        {
            return await _httpClient.GetFromJsonAsync<List<StateDto>>($"api/state/by-country/{countryId}") ?? new();

        }


        ///Pagination
        ///
        public async Task<PagedResult<EventDto>> GetPagedEventsAsync(PagedRequest req)
        {
            await AddAuthHeaderAsync();

            // 1️⃣ Convert "Approve" → "AdminApproved"
            string status = req.Status;
            if (!string.IsNullOrWhiteSpace(req.Status))
            {
                status = req.Status switch
                {
                    "Approve" => "AdminApproved",
                    "Reject" => "Rejected",
                    _ => req.Status
                };
            }

            //  Format date properly (Send only the date part)
            string dateFilter = req.DateFilter.HasValue
                ? req.DateFilter.Value.ToString("yyyy-MM-dd")
                : string.Empty;

            //  Build query string
            var query = $"api/Events/paged?" +
                        $"page={req.Page}" +
                        $"&pageSize={req.PageSize}" +
                        $"&search={req.Search ?? ""}" +
                        $"&status={status ?? ""}" +
                        $"&dateFilter={dateFilter}";

            //  Call API and parse the response
            var result = await _httpClient.GetFromJsonAsync<PagedResult<EventDto>>(query);

            return result ?? new PagedResult<EventDto>();
        }

    }
}
