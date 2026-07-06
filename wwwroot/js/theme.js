// Theme management with localStorage persistence
(function() {
    const THEME_KEY = 'theme-preference';
    const THEME_ATTRIBUTE = 'data-theme';

    // Get theme from localStorage or system preference
    function getThemePreference() {
        const stored = localStorage.getItem(THEME_KEY);
        if (stored) {
            return stored === 'dark';
        }
        return window.matchMedia('(prefers-color-scheme: dark)').matches;
    }

    // Apply theme to document
    function applyTheme(isDark) {
        document.documentElement.setAttribute(THEME_ATTRIBUTE, isDark ? 'dark' : 'light');
        localStorage.setItem(THEME_KEY, isDark ? 'dark' : 'light');
    }

    // Initialize theme on load
    applyTheme(getThemePreference());

    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
        if (!localStorage.getItem(THEME_KEY)) {
            applyTheme(e.matches);
        }
    });

    // Expose functions to Blazor
    window.theme = {
        getTheme: function() {
            return getThemePreference();
        },
        toggleTheme: function() {
            const current = getThemePreference();
            const newTheme = !current;
            applyTheme(newTheme);
            return newTheme;
        },
        setTheme: function(isDark) {
            applyTheme(isDark);
        }
    };
})();
