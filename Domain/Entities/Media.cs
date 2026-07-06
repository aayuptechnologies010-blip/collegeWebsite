using CollegeWebSite.Domain.Enums;

namespace CollegeWebSite.Domain.Entities;

public class Media : AuditableEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty; // Adding Url for convenience
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? AltText { get; set; }
    public string? Description { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public MediaType Type { get; set; }
}
