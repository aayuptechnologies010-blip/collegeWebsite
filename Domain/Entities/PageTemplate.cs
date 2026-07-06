namespace CollegeWebSite.Domain.Entities;

public class PageTemplate : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LayoutJson { get; set; } = string.Empty; // JSON schema defining zones/blocks
    public bool IsSystemTemplate { get; set; } = false; // Cannot be deleted if true

    // Navigation properties
    public virtual ICollection<PageContent> Pages { get; set; } = new List<PageContent>();
}
