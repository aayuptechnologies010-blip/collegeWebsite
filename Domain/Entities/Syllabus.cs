namespace CollegeWebSite.Domain.Entities;

public class Syllabus : AuditableEntity
{
    public string CourseName { get; set; } = string.Empty; // e.g., GNM Nursing, ANM
    public string Subject { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty; // e.g., 1st Year, 2nd Year
    public string Description { get; set; } = string.Empty;
    public long? DocumentMediaId { get; set; }
    public Media? DocumentMedia { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
