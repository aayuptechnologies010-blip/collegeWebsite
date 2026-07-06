namespace CollegeWebSite.Domain.Settings;

public class InstitutionSettings
{
    public string AcademicYear { get; set; } = string.Empty;
    public bool AdmissionsOpen { get; set; }
    public PrincipalSettings Principal { get; set; } = new();
    public ContactSettings Contact { get; set; } = new();
}

public class PrincipalSettings
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class ContactSettings
{
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string GoogleMapsUrl { get; set; } = string.Empty;
    public string NavigationUrl { get; set; } = string.Empty;
    public string FacebookUrl { get; set; } = string.Empty;
    public string InstagramUrl { get; set; } = string.Empty;
}
