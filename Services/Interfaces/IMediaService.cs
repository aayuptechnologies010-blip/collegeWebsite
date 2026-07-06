using CollegeWebSite.Domain.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace CollegeWebSite.Services.Interfaces;

public interface IMediaService
{
    Task<List<Media>> GetAllMediaAsync();
    Task<Media?> GetMediaByIdAsync(long id);
    Task<Media?> UploadMediaAsync(IBrowserFile file, string? altText = null, string? description = null);
    Task<bool> DeleteMediaAsync(long id);
    Task<bool> UpdateMediaInfoAsync(long id, string? altText, string? description);
    Task<int> GetTotalMediaCountAsync();
}
