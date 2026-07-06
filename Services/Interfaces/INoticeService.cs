using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface INoticeService
{
    Task<List<CampusNotice>> GetActiveNoticesAsync();
    Task<List<CampusNotice>> GetAllNoticesAsync();
    Task<CampusNotice?> GetNoticeByIdAsync(long id);
    Task<bool> UpsertNoticeAsync(CampusNotice notice);
    Task<bool> DeleteNoticeAsync(long id);
}
