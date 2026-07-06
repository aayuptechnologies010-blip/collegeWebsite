using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class SyllabusService : ISyllabusService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public SyllabusService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Syllabus>> GetAllSyllabusesAsync(bool onlyActive = false)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Syllabuses.Include(s => s.DocumentMedia).AsQueryHeader();
        
        if (onlyActive)
        {
            query = query.Where(s => s.IsActive);
        }

        return await query.OrderBy(s => s.CourseName).ThenBy(s => s.DisplayOrder).ToListAsync();
    }

    public async Task<List<Syllabus>> GetSyllabusesByCourseAsync(string courseName, bool onlyActive = false)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Syllabuses.Include(s => s.DocumentMedia).Where(s => s.CourseName == courseName);
        
        if (onlyActive)
        {
            query = query.Where(s => s.IsActive);
        }

        return await query.OrderBy(s => s.DisplayOrder).ToListAsync();
    }

    public async Task<Syllabus?> GetSyllabusByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Syllabuses.Include(s => s.DocumentMedia).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> UpsertSyllabusAsync(Syllabus syllabus)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (syllabus.Id == 0)
        {
            syllabus.CreatedOn = DateTime.UtcNow;
            context.Syllabuses.Add(syllabus);
        }
        else
        {
            syllabus.LastModifiedOn = DateTime.UtcNow;
            context.Syllabuses.Update(syllabus);
        }

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteSyllabusAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var syllabus = await context.Syllabuses.FindAsync(id);
        if (syllabus == null) return false;

        syllabus.DeletedOn = DateTime.UtcNow;
        context.Syllabuses.Update(syllabus);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<List<string>> GetAvailableCoursesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Syllabuses
            .Select(s => s.CourseName)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}

public static class QueryExtensions 
{
    public static IQueryable<T> AsQueryHeader<T>(this IQueryable<T> query) where T : class => query.AsNoTracking();
}
