namespace CollegeWebSite.Domain.Enums;

public enum PopUpType
{
    Modal = 0,    // Center dialog with overlay
    Banner = 1,   // Top/Bottom persistent bar
    Toast = 2     // Small floating notification
}

public enum PopUpFrequency
{
    OncePerUser = 0,   // Browsing history/localstorage based
    OncePerSession = 1, // Session storage based
    OncePerDay = 2,     // Cookie with 24h expiry
    Always = 3          // Every page load (within rules)
}
