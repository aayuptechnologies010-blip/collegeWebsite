using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class PageService : IPageService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<PageService> _logger;

    public PageService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<PageService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<PageContent?> GetPageBySlugAsync(string slug)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PageContents
            .Include(p => p.Seo)
            .Include(p => p.Template)
            .AsNoTracking() // Performance optimization for read-only
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
    }

    public async Task<List<PageContent>> GetAllPagesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PageContents
            .Include(p => p.Template)
            .OrderBy(p => p.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PageContent> CreatePageAsync(PageContent page)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Ensure slug is unique
        if (await context.PageContents.AnyAsync(p => p.Slug == page.Slug))
        {
            throw new InvalidOperationException($"Page with slug '{page.Slug}' already exists.");
        }

        context.PageContents.Add(page);
        await context.SaveChangesAsync();
        return page;
    }

    public async Task<PageContent> UpdatePageAsync(PageContent page)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.PageContents.Update(page);
        await context.SaveChangesAsync();
        return page;
    }

    public async Task DeletePageAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var page = await context.PageContents.FindAsync(id);
        if (page != null)
        {
            // Soft delete handled by AuditableEntity logic if implemented, otherwise hard delete here or set flag
            // Assuming soft delete via query filter is handled, we just need to set DeletedOn
            page.DeletedOn = DateTime.UtcNow;
            // page.DeletedBy = ... context user
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PageContents.AnyAsync(p => p.Slug == slug);
    }
}
