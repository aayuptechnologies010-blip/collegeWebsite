using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class NoticeService : INoticeService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public NoticeService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<CampusNotice>> GetActiveNoticesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;
        return await context.CampusNotices
            .Where(n => n.IsActive && (n.ExpiryDate == null || n.ExpiryDate > now))
            .OrderByDescending(n => n.IsUrgent)
            .ThenByDescending(n => n.CreatedOn)
            .ToListAsync();
    }

    public async Task<List<CampusNotice>> GetAllNoticesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CampusNotices
            .OrderByDescending(n => n.CreatedOn)
            .ToListAsync();
    }

    public async Task<CampusNotice?> GetNoticeByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CampusNotices.FindAsync(id);
    }

    public async Task<bool> UpsertNoticeAsync(CampusNotice notice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (notice.Id == 0)
        {
            await context.CampusNotices.AddAsync(notice);
        }
        else
        {
            context.CampusNotices.Update(notice);
        }
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteNoticeAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var notice = await context.CampusNotices.FindAsync(id);
        if (notice != null)
        {
            context.CampusNotices.Remove(notice);
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
