using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class FacultyService : IFacultyService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public FacultyService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Faculty>> GetAllFacultyAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Faculty
            .Include(f => f.Photo)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Faculty?> GetFacultyByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Faculty
            .Include(f => f.Photo)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<bool> UpsertFacultyAsync(Faculty faculty)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (faculty.Id == 0)
        {
            await context.Faculty.AddAsync(faculty);
        }
        else
        {
            context.Faculty.Update(faculty);
        }
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteFacultyAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var faculty = await context.Faculty.FindAsync(id);
        if (faculty != null)
        {
            context.Faculty.Remove(faculty);
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
