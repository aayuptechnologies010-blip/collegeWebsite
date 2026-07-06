namespace CollegeWebSite.Domain.Entities;

public class PageSeo : BaseEntity
{
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
    public string? TwitterCard { get; set; }
    public string? CanonicalUrl { get; set; }

    // Navigation properties
    public virtual PageContent? PageContent { get; set; }
}
