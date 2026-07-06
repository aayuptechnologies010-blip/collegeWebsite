window.popupManager = {
    checkFrequency: function (id, frequency) {
        const now = new Date().getTime();
        const key = `ishika_popup_${id}`;
        
        switch (frequency) {
            case 0: // OncePerUser (Always skip if seen before)
                return !localStorage.getItem(key);
            case 1: // OncePerSession
                return !sessionStorage.getItem(key);
            case 2: // OncePerDay
                const lastSeen = localStorage.getItem(key);
                if (!lastSeen) return true;
                const diff = (now - parseInt(lastSeen)) / (1000 * 60 * 60);
                return diff >= 24;
            default: // Always
                return true;
        }
    },
    markSeen: function (id, frequency) {
        const now = new Date().getTime().toString();
        const key = `ishika_popup_${id}`;
        
        if (frequency === 1) {
            sessionStorage.setItem(key, now);
        } else {
            localStorage.setItem(key, now);
        }
    },
    getMaxZIndex: function () {
        return Math.max(
            ...Array.from(document.querySelectorAll('body *'), el =>
                parseFloat(window.getComputedStyle(el).zIndex)
            ).filter(zIndex => !isNaN(zIndex)),
            1000 // Default minimum
        );
    }
};
