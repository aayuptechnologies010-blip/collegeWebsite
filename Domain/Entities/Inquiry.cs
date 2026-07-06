using System.ComponentModel.DataAnnotations;
using CollegeWebSite.Domain.Enums;

namespace CollegeWebSite.Domain.Entities;

public class Inquiry : AuditableEntity
{
    [Required(ErrorMessage = "ENTER YOUR FULL NAME")]
    [StringLength(100, ErrorMessage = "NAME IS TOO LONG")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "EMAIL ADDRESS IS MANDATORY")]
    [EmailAddress(ErrorMessage = "INVALID EMAIL FORMAT")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PHONE NUMBER IS REQUIRED")]
    [Phone(ErrorMessage = "INVALID PHONE NUMBER")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "SUBJECT IS REQUIRED")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "PLEASE PROVIDE A MESSAGE OR COURSE PREFERENCE")]
    public string Message { get; set; } = string.Empty;

    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
    public long? AssignedToUserId { get; set; } // FK to User (optional)
    public string? Notes { get; set; }

    // Navigation properties
    public virtual User? AssignedToUser { get; set; }
}
