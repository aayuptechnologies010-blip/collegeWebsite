using CollegeWebSite.Domain.Enums;

namespace CollegeWebSite.Domain.Entities;

public class AnnouncementPopUp : AuditableEntity
{
    // Content data
    public string Title { get; set; } = string.Empty;
    public string ContentHtml { get; set; } = string.Empty; // Rich Text SUPPORTED
    public string? ImageUrl { get; set; } // Managed from Media Vault
    public string? IconName { get; set; } // Optional lucide-react or SVG name
    
    // Call-To-Action (Primary)
    public string? PrimaryButtonText { get; set; }
    public string? PrimaryButtonUrl { get; set; }
    
    // Auxiliary Button (Secondary)
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }

    // Display Rules
    public PopUpType Type { get; set; } = PopUpType.Modal;
    public int Priority { get; set; } = 1; // Higher = showed first
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Target Pages (Comma separated slugs, e.g. "index,courses,about")
    public string? TargetPages { get; set; } = "*"; // * for all pages

    // Device Targeting
    public bool ShowOnMobile { get; set; } = true;
    public bool ShowOnDesktop { get; set; } = true;

    // User Experience
    public int DelaySeconds { get; set; } = 0; // Show after X seconds
    public PopUpFrequency Frequency { get; set; } = PopUpFrequency.OncePerSession;
    
    // Performance Analytics
    public int Impressions { get; set; } = 0;
    public int Clicks { get; set; } = 0;
    public int Dismissals { get; set; } = 0;
}
