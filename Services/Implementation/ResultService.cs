using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Services.Implementation;

public class ResultService : IResultService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<ResultService> _logger;

    public ResultService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<ResultService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<StudentResult?> GetResultByRollNumberAsync(string rollNumber, string examName)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var result = await context.StudentResults
            .Where(r => r.IsPublished)
            .FirstOrDefaultAsync(r => r.RollNumber == rollNumber && r.ExamName == examName);

        if (result != null)
        {
            _logger.LogInformation("Academic Audit: Result verified for Roll No {RollNumber} in {ExamName}. Outcome: {Status}", rollNumber, examName, result.ResultStatus);
        }
        else
        {
            _logger.LogWarning("Security Alert: Unsuccessful result verification attempted for Roll No {RollNumber} in {ExamName}", rollNumber, examName);
        }

        return result;
    }

    public async Task<List<StudentResult>> GetAllResultsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StudentResults
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();
    }

    public async Task<StudentResult?> GetResultByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StudentResults.FindAsync(id);
    }

    public async Task<bool> UpsertResultAsync(StudentResult result)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (result.Id == 0)
        {
            await context.StudentResults.AddAsync(result);
        }
        else
        {
            context.StudentResults.Update(result);
        }
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteResultAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var result = await context.StudentResults.FindAsync(id);
        if (result != null)
        {
            context.StudentResults.Remove(result);
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }

    public async Task<List<string>> GetAvailableExamsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StudentResults
            .Where(r => r.IsPublished)
            .Select(r => r.ExamName)
            .Distinct()
            .ToListAsync();
    }
}
