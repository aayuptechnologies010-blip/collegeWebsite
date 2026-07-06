namespace CollegeWebSite.Domain.Entities;

public class CampusNotice : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // Brief summary
    public string? LinkUrl { get; set; } // Optional link for more details
    public bool IsUrgent { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiryDate { get; set; }
}
