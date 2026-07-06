using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IFacultyService
{
    Task<List<Faculty>> GetAllFacultyAsync();
    Task<Faculty?> GetFacultyByIdAsync(long id);
    Task<bool> UpsertFacultyAsync(Faculty faculty);
    Task<bool> DeleteFacultyAsync(long id);
}
