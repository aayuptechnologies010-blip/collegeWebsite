namespace CollegeWebSite.Domain.Entities;

public class Event : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Venue { get; set; }
    public long? BannerMediaId { get; set; } // FK to Media
    public bool IsPublished { get; set; }
    public DateTime? PublishedOn { get; set; }

    // Navigation properties
    public virtual Media? Banner { get; set; }
}
