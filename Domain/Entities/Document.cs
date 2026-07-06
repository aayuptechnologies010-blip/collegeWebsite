using CollegeWebSite.Domain.Enums;

namespace CollegeWebSite.Domain.Entities;

public class Document : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long MediaId { get; set; } // FK to Media
    public DocumentType Type { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsPublic { get; set; }

    // Navigation properties
    public virtual Media Media { get; set; } = null!;
}
