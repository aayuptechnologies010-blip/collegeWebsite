using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CollegeWebSite.Services.Implementation;

public class HeroService : IHeroService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IMemoryCache _cache;
    private const string HeroCacheKey = "Ishika_Hero_Content";

    public HeroService(IDbContextFactory<ApplicationDbContext> contextFactory, IMemoryCache cache)
    {
        _contextFactory = contextFactory;
        _cache = cache;
    }

    public async Task<HeroContent?> GetActiveHeroContentAsync()
    {
        if (_cache.TryGetValue(HeroCacheKey, out HeroContent? cachedContent))
        {
            return cachedContent;
        }

        using var context = _contextFactory.CreateDbContext();
        var content = await context.HeroSectionContent
            .OrderByDescending(h => h.IsActive)
            .ThenByDescending(h => h.CreatedOn)
            .FirstOrDefaultAsync();

        if (content != null)
        {
            _cache.Set(HeroCacheKey, content, TimeSpan.FromHours(12));
        }

        return content;
    }

    public async Task<bool> UpdateHeroContentAsync(HeroContent content)
    {
        using var context = _contextFactory.CreateDbContext();
        
        if (content.Id > 0)
        {
            context.HeroSectionContent.Update(content);
        }
        else 
        {
            await context.HeroSectionContent.AddAsync(content);
        }

        var result = await context.SaveChangesAsync() > 0;
        
        if (result)
        {
            _cache.Remove(HeroCacheKey);
        }

        return result;
    }
}
