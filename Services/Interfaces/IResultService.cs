using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IResultService
{
    Task<StudentResult?> GetResultByRollNumberAsync(string rollNumber, string examName);
    Task<List<StudentResult>> GetAllResultsAsync();
    Task<StudentResult?> GetResultByIdAsync(long id);
    Task<bool> UpsertResultAsync(StudentResult result);
    Task<bool> DeleteResultAsync(long id);
    Task<List<string>> GetAvailableExamsAsync();
}
