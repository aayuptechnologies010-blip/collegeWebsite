using System.Text.Json;
using System.Text.Json.Serialization;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;
using CollegeWebSite.Domain.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace CollegeWebSite.Services.Implementation;

public class SeoService : ISeoService
{
    private readonly NavigationManager _navigationManager;
    private readonly InstitutionSettings _institution;
    private readonly SeoSettings _seo;

    public SeoService(
        NavigationManager navigationManager,
        IOptions<InstitutionSettings> institutionOptions,
        IOptions<SeoSettings> seoOptions)
    {
        _navigationManager = navigationManager;
        _institution = institutionOptions.Value;
        _seo = seoOptions.Value;
    }

    private static readonly JsonSerializerOptions JsonLdOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string GenerateWebSiteSchema()
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "WebSite",
            ["name"] = _seo.SiteName,
            ["alternateName"] = _seo.OrganizationLegalName,
            ["url"] = baseUrl,
            ["description"] = _seo.DefaultDescription,
            ["inLanguage"] = "en-IN",
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _seo.OrganizationLegalName,
                ["url"] = baseUrl
            }
        };
        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }

    public string GenerateOrganizationSchema()
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var logoUrl = $"{baseUrl}/{_seo.DefaultOgImage.TrimStart('/')}";
        var sameAs = new List<string>();
        if (!string.IsNullOrWhiteSpace(_institution.Contact.FacebookUrl))
            sameAs.Add(_institution.Contact.FacebookUrl.Trim());
        if (!string.IsNullOrWhiteSpace(_institution.Contact.InstagramUrl))
            sameAs.Add(_institution.Contact.InstagramUrl.Trim());

        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Organization",
            ["name"] = _seo.OrganizationLegalName,
            ["alternateName"] = new[] { _seo.SiteName, "Ishika Nursing College" },
            ["url"] = baseUrl,
            ["logo"] = logoUrl,
            ["image"] = logoUrl,
            ["address"] = new Dictionary<string, object?>
            {
                ["@type"] = "PostalAddress",
                ["streetAddress"] = _institution.Contact.Address,
                ["addressLocality"] = "Kendujhar",
                ["addressRegion"] = "Odisha",
                ["postalCode"] = "758001",
                ["addressCountry"] = "IN"
            },
            ["contactPoint"] = new Dictionary<string, object?>
            {
                ["@type"] = "ContactPoint",
                ["telephone"] = _institution.Contact.Phone,
                ["contactType"] = "Admissions",
                ["email"] = _institution.Contact.Email,
                ["areaServed"] = "IN",
                ["availableLanguage"] = new[] { "English", "Odia", "Hindi" }
            }
        };
        if (sameAs.Count > 0)
            schema["sameAs"] = sameAs.ToArray();

        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }

    public string GenerateEducationalInstitutionSchema()
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "EducationalOrganization",
            ["name"] = _seo.OrganizationLegalName,
            ["alternateName"] = _seo.SiteName,
            ["description"] = "Private nursing and paramedical institution in Kendujhar, Odisha — programs recognized by relevant councils with modern clinical training.",
            ["url"] = baseUrl,
            ["address"] = new Dictionary<string, object?>
            {
                ["@type"] = "PostalAddress",
                ["streetAddress"] = _institution.Contact.Address,
                ["addressLocality"] = "Kendujhar",
                ["addressRegion"] = "Odisha",
                ["postalCode"] = "758001",
                ["addressCountry"] = "IN"
            },
            ["telephone"] = _institution.Contact.Phone,
            ["email"] = _institution.Contact.Email
        };
        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }

    public string GenerateCourseSchema(string courseName, string description)
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Course",
            ["name"] = courseName,
            ["description"] = description,
            ["provider"] = new Dictionary<string, object?>
            {
                ["@type"] = "EducationalOrganization",
                ["name"] = _seo.OrganizationLegalName,
                ["url"] = baseUrl
            },
            ["courseCode"] = "GNM",
            ["educationalCredentialAwarded"] = "GNM Certificate",
            ["numberOfCredits"] = new Dictionary<string, object?>
            {
                ["@type"] = "QuantitativeValue",
                ["value"] = "3",
                ["unitText"] = "Years"
            }
        };
        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }

    public string GeneratePersonSchema(Faculty faculty)
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Person",
            ["name"] = faculty.Name,
            ["jobTitle"] = faculty.Designation ?? "Faculty Member",
            ["description"] = faculty.Bio,
            ["email"] = faculty.Email,
            ["telephone"] = faculty.PhoneNumber,
            ["worksFor"] = new Dictionary<string, object?>
            {
                ["@type"] = "EducationalOrganization",
                ["name"] = _seo.OrganizationLegalName,
                ["url"] = baseUrl
            }
        };
        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }

    public string GenerateEventSchema(Event eventEntity)
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Event",
            ["name"] = eventEntity.Title,
            ["description"] = eventEntity.Description,
            ["startDate"] = eventEntity.EventDate.ToString("yyyy-MM-ddTHH:mm:ss"),
            ["endDate"] = eventEntity.EndDate?.ToString("yyyy-MM-ddTHH:mm:ss") ?? eventEntity.EventDate.ToString("yyyy-MM-ddTHH:mm:ss"),
            ["eventAttendanceMode"] = "https://schema.org/OfflineEventAttendanceMode",
            ["location"] = new Dictionary<string, object?>
            {
                ["@type"] = "Place",
                ["name"] = eventEntity.Venue ?? _seo.SiteName,
                ["address"] = new Dictionary<string, object?>
                {
                    ["@type"] = "PostalAddress",
                    ["streetAddress"] = _institution.Contact.Address,
                    ["addressLocality"] = "Kendujhar",
                    ["addressRegion"] = "Odisha",
                    ["postalCode"] = "758001",
                    ["addressCountry"] = "IN"
                }
            },
            ["organizer"] = new Dictionary<string, object?>
            {
                ["@type"] = "EducationalOrganization",
                ["name"] = _seo.OrganizationLegalName,
                ["url"] = baseUrl
            }
        };
        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }

    public string GenerateBreadcrumbSchema(List<BreadcrumbItem> items)
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var listItems = items.Select((item, index) => new Dictionary<string, object?>
        {
            ["@type"] = "ListItem",
            ["position"] = index + 1,
            ["name"] = item.Name,
            ["item"] = item.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? item.Url
                : $"{baseUrl}{(item.Url.StartsWith('/') ? "" : "/")}{item.Url}"
        }).ToArray();

        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "BreadcrumbList",
            ["itemListElement"] = listItems
        };
        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }

    public string GenerateWebPageSchema(PageContent page)
    {
        var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
        var slug = string.IsNullOrWhiteSpace(page.Slug) ? "" : page.Slug.TrimStart('/');
        var pageUrl = $"{baseUrl}/p/{slug}";
        var desc = page.Seo?.MetaDescription;
        if (string.IsNullOrWhiteSpace(desc))
            desc = _seo.DefaultDescription;

        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "WebPage",
            ["name"] = page.Title,
            ["description"] = desc,
            ["url"] = pageUrl,
            ["inLanguage"] = "en-IN",
            ["isPartOf"] = new Dictionary<string, object?>
            {
                ["@type"] = "WebSite",
                ["name"] = _seo.SiteName,
                ["url"] = baseUrl
            },
            ["about"] = new Dictionary<string, object?>
            {
                ["@type"] = "EducationalOrganization",
                ["name"] = _seo.OrganizationLegalName,
                ["url"] = baseUrl
            }
        };
        return JsonSerializer.Serialize(schema, JsonLdOptions);
    }
}
