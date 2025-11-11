using System.Net.Http.Json;
using Applications.Dto;

public class ClientEventService
{
    private readonly HttpClient _http;

    public ClientEventService(HttpClient http)
    {
        _http = http;
    }

    // Get all events
    public async Task<List<EventDto>> GetAllEventsAsync()
    {
        return await _http.GetFromJsonAsync<List<EventDto>>("api/events") ?? new List<EventDto>();
    }

    // Get event by ID
    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<EventDto>($"api/events/{id}");
    }

    // Get seats for an event
    public async Task<List<SeatDto>> GetSeatsByEventIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<List<SeatDto>>($"api/events/{id}/seats") ?? new List<SeatDto>();
    }

    // Create new event
    public async Task CreateEventAsync(EventDto dto)
    {
        await _http.PostAsJsonAsync("api/events", dto);
    }

    // Update event
    public async Task UpdateEventAsync(int id, EventDto dto)
    {
        await _http.PutAsJsonAsync($"api/events/{id}", dto);
    }

    // Delete event
    public async Task DeleteEventAsync(int id)
    {
        await _http.DeleteAsync($"api/events/{id}");
    }
}

