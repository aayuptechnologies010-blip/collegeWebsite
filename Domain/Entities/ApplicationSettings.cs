namespace CollegeWebSite.Domain.Entities;

public class ApplicationSettings : BaseEntity
{
    public string Key { get; set; } = string.Empty; // Unique
    public string Value { get; set; } = string.Empty; // JSON or string
    public string? Description { get; set; }
    public string? Category { get; set; } // Theme, SEO, Email, etc.
}
