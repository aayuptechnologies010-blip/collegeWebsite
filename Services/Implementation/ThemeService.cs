using Microsoft.JSInterop;

namespace CollegeWebSite.Services.Implementation
{
    public class ThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private string _currentTheme = "classic";

        public event Action OnThemeChanged;

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public string CurrentTheme => _currentTheme;

        public async Task SetThemeAsync(string theme)
        {
            _currentTheme = theme;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
            await _jsRuntime.InvokeVoidAsync("setDocTheme", theme);
            OnThemeChanged?.Invoke();
        }

        public async Task InitializeThemeAsync()
        {
            var savedTheme = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "theme");
            if (!string.IsNullOrEmpty(savedTheme))
            {
                _currentTheme = savedTheme;
            }
            await _jsRuntime.InvokeVoidAsync("setDocTheme", _currentTheme);
        }

        public List<ThemeOption> GetThemeOptions()
        {
            return new List<ThemeOption>
            {
                new ThemeOption { Id = "classic", Name = "Classic Ishika", Primary = "#01427a", Secondary = "#e31e24", Description = "Institutional Blue & Medical Red" },
                new ThemeOption { Id = "oceanic", Name = "Deep Oceanic", Primary = "#0c4a6e", Secondary = "#0ea5e9", Description = "Professional Deep Sea Blues" },
                new ThemeOption { Id = "emerald", Name = "Emerald Forest", Primary = "#065f46", Secondary = "#10b981", Description = "Calming Medical Greenery" },
                new ThemeOption { Id = "amethyst", Name = "Royal Amethyst", Primary = "#581c87", Secondary = "#a855f7", Description = "Elegant Purple Excellence" },
                new ThemeOption { Id = "sunset", Name = "Sunset Ember", Primary = "#9f1239", Secondary = "#f43f5e", Description = "Vibrant Rose & Deep Crimson" },
                new ThemeOption { Id = "midnight", Name = "Midnight Slate", Primary = "#0f172a", Secondary = "#334155", Description = "Sleek Dark Command Center" }
            };
        }
    }

    public class ThemeOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Primary { get; set; }
        public string Secondary { get; set; }
        public string Description { get; set; }
    }
}
