using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IGalleryService
{
    Task<List<GalleryCategory>> GetCategoriesWithImagesAsync();
    Task<List<GalleryImage>> GetImagesByCategoryAsync(long categoryId);
    Task<List<GalleryImage>> GetRecentImagesAsync(int count);
    
    // Administrative Actions
    Task<List<GalleryCategory>> GetAllCategoriesAsync();
    Task<bool> UpsertCategoryAsync(GalleryCategory category);
    Task<bool> DeleteCategoryAsync(long id);
    Task<bool> AddImageToGalleryAsync(GalleryImage image);
    Task<bool> DeleteImageAsync(long id);
    Task<GalleryCategory?> GetCategoryByIdAsync(long id);
}
