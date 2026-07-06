namespace CollegeWebSite.Domain.Settings;

/// <summary>
/// Site-wide SEO defaults (titles, social, robots). Override per-page via SeoHead parameters.
/// </summary>
public class SeoSettings
{
    public string SiteName { get; set; } = "Ishika Nursing & Paramedical Studies";
    public string OrganizationLegalName { get; set; } = "ISHIKA NURSING AND PARAMEDICAL STUDIES";
    public string DefaultTitleSuffix { get; set; } = "Ishika Nursing & Paramedical Studies";
    public string DefaultDescription { get; set; } =
        "Odisha's premier nursing institution in Kendujhar. GNM, ANM, B.Sc Nursing & paramedical programs with modern labs and placement support.";
    public string DefaultKeywords { get; set; } =
        "Nursing College Odisha, GNM Course, Paramedical Studies, Ishika Nursing Kendujhar, Keonjhar Nursing, Medical Education Odisha, Ishika Nursing College";
    /// <summary>Path under wwwroot for default Open Graph / Twitter image.</summary>
    public string DefaultOgImage { get; set; } = "favicon.png";
    /// <summary>Twitter/X handle without @ (e.g. ishikanursingcollege).</summary>
    public string? TwitterSite { get; set; }
    public string Locale { get; set; } = "en_IN";
    public string OgLocale { get; set; } = "en_IN";
    public string GeoRegion { get; set; } = "IN-OR";
    public string GeoPlacename { get; set; } = "Kendujhar";
    public string RobotsDefault { get; set; } = "index, follow";
    public string ThemeColor { get; set; } = "#01427a";
    public string Author { get; set; } = "Ishika Nursing & Paramedical Studies";
}
