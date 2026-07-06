using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface ISyllabusService
{
    Task<List<Syllabus>> GetAllSyllabusesAsync(bool onlyActive = false);
    Task<List<Syllabus>> GetSyllabusesByCourseAsync(string courseName, bool onlyActive = false);
    Task<Syllabus?> GetSyllabusByIdAsync(long id);
    Task<bool> UpsertSyllabusAsync(Syllabus syllabus);
    Task<bool> DeleteSyllabusAsync(long id);
    Task<List<string>> GetAvailableCoursesAsync();
}
