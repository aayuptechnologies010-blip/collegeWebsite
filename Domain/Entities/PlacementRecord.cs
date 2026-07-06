namespace CollegeWebSite.Domain.Entities;

public class PlacementRecord : AuditableEntity
{
    public string StudentName { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty; // GNM, ANM, B.Sc
    public string HospitalName { get; set; } = string.Empty; // e.g., Apollo, AMRI, Fortis
    public string Designation { get; set; } = string.Empty; // Staff Nurse, Lab Tech
    public string? Batch { get; set; } // e.g., 2021-24
    public long? StudentPhotoId { get; set; } // FK to Media
    public string? Testimonial { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual Media? StudentPhoto { get; set; }
}
