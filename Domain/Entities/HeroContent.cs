namespace CollegeWebSite.Domain.Entities;

public class HeroContent : AuditableEntity
{
    public string WelcomeBadge { get; set; } = "ADMISSIONS OPEN";
    
    public string HeadlineMain { get; set; } = "Crafting the Future of Care";
    public string HeadlineShine { get; set; } = "at Ishika Campus";
    
    public string SubHeadline { get; set; } = "Step into a world of clinical precision and compassionate service.";
    
    public string PrimaryButtonText { get; set; } = "Begin Application";
    public string PrimaryButtonLink { get; set; } = "/apply";
    
    public string SecondaryButtonText { get; set; } = "Explore Curriculum";
    public string SecondaryButtonLink { get; set; } = "/p/courses";
    
    // Stats
    public string Stat1Value { get; set; } = "500+";
    public string Stat1Label { get; set; } = "Elite Alumni";
    
    public string Stat2Value { get; set; } = "100%";
    public string Stat2Label { get; set; } = "Placement";
    
    public string Stat3Value { get; set; } = "A+ Grade";
    public string Stat3Label { get; set; } = "ONMRC";
    
    // Floating Badge
    public string BadgeStatus { get; set; } = "Authentic";
    public string BadgeLabel { get; set; } = "Govt Recognized";
    
    public string BannerImagePath { get; set; } = "images/banner.jpeg";
    
    public bool IsActive { get; set; } = true;
}
