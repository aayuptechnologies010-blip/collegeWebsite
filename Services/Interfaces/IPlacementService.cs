using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IPlacementService
{
    Task<List<PlacementRecord>> GetAllPlacementsAsync(bool featuredOnly = false);
    Task<PlacementRecord?> GetPlacementByIdAsync(long id);
    Task<bool> UpsertPlacementAsync(PlacementRecord placement);
    Task<bool> DeletePlacementAsync(long id);
}
