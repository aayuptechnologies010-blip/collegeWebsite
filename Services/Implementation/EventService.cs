using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class EventService : IEventService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public EventService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Event>> GetAllEventsAsync(bool onlyPublished = true)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Events.Include(e => e.Banner).AsQueryable();
        
        if (onlyPublished)
        {
            query = query.Where(e => e.IsPublished);
        }
        
        return await query.OrderByDescending(e => e.EventDate).ToListAsync();
    }

    public async Task<List<Event>> GetUpcomingEventsAsync(int count = 3)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Include(e => e.Banner)
            .Where(e => e.IsPublished && e.EventDate >= DateTime.UtcNow.Date)
            .OrderBy(e => e.EventDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Include(e => e.Banner)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> UpsertEventAsync(Event @event)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (@event.Id == 0)
        {
            await context.Events.AddAsync(@event);
        }
        else
        {
            context.Events.Update(@event);
        }
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteEventAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var @event = await context.Events.FindAsync(id);
        if (@event != null)
        {
            context.Events.Remove(@event);
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
