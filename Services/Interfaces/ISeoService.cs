using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface ISeoService
{
    /// <summary>
    /// Generates JSON-LD for WebSite (name, url, publisher).
    /// </summary>
    string GenerateWebSiteSchema();

    /// <summary>
    /// Generates JSON-LD structured data for Organization
    /// </summary>
    string GenerateOrganizationSchema();

    /// <summary>
    /// Generates JSON-LD structured data for Educational Institution
    /// </summary>
    string GenerateEducationalInstitutionSchema();

    /// <summary>
    /// Generates JSON-LD structured data for Course
    /// </summary>
    string GenerateCourseSchema(string courseName, string description);

    /// <summary>
    /// Generates JSON-LD structured data for Person (Faculty)
    /// </summary>
    string GeneratePersonSchema(Faculty faculty);

    /// <summary>
    /// Generates JSON-LD structured data for Event
    /// </summary>
    string GenerateEventSchema(Event eventEntity);

    /// <summary>
    /// Generates JSON-LD structured data for BreadcrumbList
    /// </summary>
    string GenerateBreadcrumbSchema(List<BreadcrumbItem> items);

    /// <summary>
    /// Generates JSON-LD structured data for WebPage
    /// </summary>
    string GenerateWebPageSchema(PageContent page);
}

public class BreadcrumbItem
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
