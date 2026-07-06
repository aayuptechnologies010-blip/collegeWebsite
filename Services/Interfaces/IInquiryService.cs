using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IInquiryService
{
    Task<Inquiry> CreateInquiryAsync(Inquiry inquiry);
    Task<List<Inquiry>> GetAllInquiriesAsync();
    Task<Inquiry> GetInquiryByIdAsync(long id);
    Task<bool> UpdateInquiryStatusAsync(long id, Domain.Enums.InquiryStatus status, string? notes = null);
    Task<int> GetTotalInquiriesCountAsync();
    Task<List<Inquiry>> GetRecentInquiriesAsync(int count);
    Task<bool> DeleteInquiryAsync(long id);
}
