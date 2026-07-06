namespace CollegeWebSite.Domain.Entities;

public class GalleryImage : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long MediaId { get; set; } // FK to Media
    public long? GalleryCategoryId { get; set; } // FK to GalleryCategory
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual Media Media { get; set; } = null!;
    public virtual GalleryCategory? Category { get; set; }
}
