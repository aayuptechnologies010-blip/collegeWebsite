using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IEventService
{
    Task<List<Event>> GetAllEventsAsync(bool onlyPublished = true);
    Task<List<Event>> GetUpcomingEventsAsync(int count = 3);
    Task<Event?> GetEventByIdAsync(long id);
    Task<bool> UpsertEventAsync(Event @event);
    Task<bool> DeleteEventAsync(long id);
}
