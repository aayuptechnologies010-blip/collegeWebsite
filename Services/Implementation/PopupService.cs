using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CollegeWebSite.Services.Implementation;

public class PopupService : IPopupService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PopupService> _logger;
    private const string CACHE_KEY = "PopUp_Active";

    public PopupService(IDbContextFactory<ApplicationDbContext> contextFactory, IMemoryCache cache, ILogger<PopupService> logger)
    {
        _contextFactory = contextFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<AnnouncementPopUp>> GetAllPopupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AnnouncementPopUps
            .OrderByDescending(p => p.Priority)
            .ThenByDescending(p => p.CreatedOn)
            .ToListAsync();
    }

    public async Task<AnnouncementPopUp?> GetPopupByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AnnouncementPopUps.FindAsync(id);
    }

    public async Task<bool> CreatePopupAsync(AnnouncementPopUp popup)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        popup.CreatedOn = DateTime.UtcNow;
        context.AnnouncementPopUps.Add(popup);
        var success = await context.SaveChangesAsync() > 0;
        if (success) _cache.Remove(CACHE_KEY);
        return success;
    }

    public async Task<bool> UpdatePopupAsync(AnnouncementPopUp popup)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.AnnouncementPopUps.Update(popup);
        var success = await context.SaveChangesAsync() > 0;
        if (success) _cache.Remove(CACHE_KEY);
        return success;
    }

    public async Task<bool> DeletePopupAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var popup = await context.AnnouncementPopUps.FindAsync(id);
        if (popup == null) return false;

        popup.DeletedOn = DateTime.UtcNow; // Soft delete
        var success = await context.SaveChangesAsync() > 0;
        if (success) _cache.Remove(CACHE_KEY);
        return success;
    }

    public async Task<List<AnnouncementPopUp>> GetRelevantPopupsAsync(string currentUri, bool isMobile)
    {
        // Use Cache to avoid DB pressure on every page load
        if (!_cache.TryGetValue(CACHE_KEY, out List<AnnouncementPopUp>? activePopups))
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var now = DateTime.UtcNow;
            
            activePopups = await context.AnnouncementPopUps
                .Where(p => p.IsActive)
                .Where(p => (p.StartDate == null || p.StartDate <= now) && (p.EndDate == null || p.EndDate >= now))
                .OrderByDescending(p => p.Priority)
                .ToListAsync();

            _cache.Set(CACHE_KEY, activePopups, TimeSpan.FromHours(1));
        }

        if (activePopups == null) return new List<AnnouncementPopUp>();

        // Filter based on delivery rules (device, page)
        return activePopups.Where(p => {
                // Device Targeting
                if (isMobile && !p.ShowOnMobile) return false;
                if (!isMobile && !p.ShowOnDesktop) return false;

                // Page Targeting
                if (string.IsNullOrEmpty(p.TargetPages) || p.TargetPages == "*") return true;

                var slugs = p.TargetPages.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower());
                var currentPath = new Uri(currentUri).AbsolutePath.ToLower();
                if (currentPath == "/" || string.IsNullOrEmpty(currentPath)) currentPath = "index";
                else currentPath = currentPath.TrimStart('/');

                return slugs.Any(s => currentPath.Contains(s));
            })
            .ToList();
    }

    public async Task RecordImpressionAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.AnnouncementPopUps.Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Impressions, p => p.Impressions + 1));
    }

    public async Task RecordClickAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.AnnouncementPopUps.Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Clicks, p => p.Clicks + 1));
    }

    public async Task RecordDismissalAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.AnnouncementPopUps.Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Dismissals, p => p.Dismissals + 1));
    }
}
