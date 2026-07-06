using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IPopupService
{
    // Management
    Task<List<AnnouncementPopUp>> GetAllPopupsAsync();
    Task<AnnouncementPopUp?> GetPopupByIdAsync(long id);
    Task<bool> CreatePopupAsync(AnnouncementPopUp popup);
    Task<bool> UpdatePopupAsync(AnnouncementPopUp popup);
    Task<bool> DeletePopupAsync(long id);
    
    // Delivery (with Caching)
    Task<List<AnnouncementPopUp>> GetRelevantPopupsAsync(string currentUri, bool isMobile);
    
    // Analytics
    Task RecordImpressionAsync(long id);
    Task RecordClickAsync(long id);
    Task RecordDismissalAsync(long id);
}
