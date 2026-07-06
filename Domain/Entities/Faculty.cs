namespace CollegeWebSite.Domain.Entities;

public class Faculty : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string? Qualification { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public long? PhotoMediaId { get; set; } // FK to Media
    public string? Bio { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual Media? Photo { get; set; }
}
