namespace CollegeWebSite.Domain.Entities;

public class StudentResult : AuditableEntity
{
    public string RollNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty; // e.g., 1st Year Annual Exam, Semester II
    public string Course { get; set; } = string.Empty; // GNM, ANM
    public string Batch { get; set; } = string.Empty;
    public string ResultStatus { get; set; } = string.Empty; // Pass, Fail, Supple
    public double TotalPercentage { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? DownloadUrl { get; set; } // Link to PDF marksheet if available
    public bool IsPublished { get; set; }
}
