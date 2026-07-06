namespace CollegeWebSite.Domain.Entities;

public class GalleryCategory : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual ICollection<GalleryImage> Images { get; set; } = new List<GalleryImage>();
}
