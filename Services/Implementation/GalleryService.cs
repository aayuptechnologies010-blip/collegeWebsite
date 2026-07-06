using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class GalleryService : IGalleryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public GalleryService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<GalleryCategory>> GetCategoriesWithImagesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GalleryCategories
            .Include(c => c.Images)
                .ThenInclude(i => i.Media)
            .ToListAsync();
    }

    public async Task<List<GalleryImage>> GetImagesByCategoryAsync(long categoryId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GalleryImages
            .Where(i => i.GalleryCategoryId == categoryId)
            .Include(i => i.Media)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<GalleryImage>> GetRecentImagesAsync(int count)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GalleryImages
            .Include(i => i.Media)
            .OrderByDescending(i => i.CreatedOn)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<GalleryCategory>> GetAllCategoriesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GalleryCategories.OrderBy(c => c.DisplayOrder).ToListAsync();
    }

    public async Task<bool> UpsertCategoryAsync(GalleryCategory category)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (category.Id == 0)
        {
            category.CreatedOn = DateTime.UtcNow;
            context.GalleryCategories.Add(category);
        }
        else
        {
            category.LastModifiedOn = DateTime.UtcNow;
            context.GalleryCategories.Update(category);
        }
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCategoryAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var cat = await context.GalleryCategories.FindAsync(id);
        if (cat == null) return false;
        cat.DeletedOn = DateTime.UtcNow;
        context.GalleryCategories.Update(cat);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddImageToGalleryAsync(GalleryImage image)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        image.CreatedOn = DateTime.UtcNow;
        context.GalleryImages.Add(image);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteImageAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var img = await context.GalleryImages.FindAsync(id);
        if (img == null) return false;
        img.DeletedOn = DateTime.UtcNow;
        context.GalleryImages.Update(img);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<GalleryCategory?> GetCategoryByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GalleryCategories.FindAsync(id);
    }
}
