namespace CollegeWebSite.Domain.Entities;

public class PageContent : AuditableEntity
{
    public string Slug { get; set; } = string.Empty; // Unique, URL-friendly
    public string Title { get; set; } = string.Empty;
    public long? TemplateId { get; set; } // FK to PageTemplate, nullable for custom layouts
    public string ContentData { get; set; } = string.Empty; // JSON per zone/block
    public long? SeoId { get; set; } // FK to PageSeo
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }

    // Navigation properties
    public virtual PageTemplate? Template { get; set; }
    public virtual PageSeo? Seo { get; set; }
}
