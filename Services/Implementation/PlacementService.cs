using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class PlacementService : IPlacementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public PlacementService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<PlacementRecord>> GetAllPlacementsAsync(bool featuredOnly = false)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.PlacementRecords.Include(p => p.StudentPhoto).AsQueryable();
        
        if (featuredOnly)
        {
            query = query.Where(p => p.IsFeatured);
        }
        
        return await query.OrderBy(p => p.DisplayOrder).ToListAsync();
    }

    public async Task<PlacementRecord?> GetPlacementByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PlacementRecords
            .Include(p => p.StudentPhoto)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> UpsertPlacementAsync(PlacementRecord placement)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (placement.Id == 0)
        {
            await context.PlacementRecords.AddAsync(placement);
        }
        else
        {
            context.PlacementRecords.Update(placement);
        }
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeletePlacementAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var placement = await context.PlacementRecords.FindAsync(id);
        if (placement != null)
        {
            context.PlacementRecords.Remove(placement);
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
