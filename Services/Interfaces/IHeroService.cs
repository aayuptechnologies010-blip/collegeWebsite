using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IHeroService
{
    Task<HeroContent?> GetActiveHeroContentAsync();
    Task<bool> UpdateHeroContentAsync(HeroContent content);
}
