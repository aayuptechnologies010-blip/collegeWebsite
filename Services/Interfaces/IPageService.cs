using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IPageService
{
    Task<PageContent?> GetPageBySlugAsync(string slug);
    Task<List<PageContent>> GetAllPagesAsync();
    Task<PageContent> CreatePageAsync(PageContent page);
    Task<PageContent> UpdatePageAsync(PageContent page);
    Task DeletePageAsync(long id);
    Task<bool> SlugExistsAsync(string slug);
}
