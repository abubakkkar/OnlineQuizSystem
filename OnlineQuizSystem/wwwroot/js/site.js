// BrainSpark Premium SaaS Interactive Scripts

document.addEventListener('DOMContentLoaded', () => {
    // 1. Dual-Theme Toggle Controller
    const themeToggleBtn = document.getElementById('themeToggle');
    if (themeToggleBtn) {
        // Retrieve current preference or default to dark
        const currentTheme = localStorage.getItem('theme') || 'dark';
        document.documentElement.setAttribute('data-theme', currentTheme);
        updateToggleIcon(currentTheme);

        themeToggleBtn.addEventListener('click', () => {
            const activeTheme = document.documentElement.getAttribute('data-theme');
            const newTheme = activeTheme === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', newTheme);
            localStorage.setItem('theme', newTheme);
            updateToggleIcon(newTheme);
        });
    }

    // 1.5. Dynamic Favicon & Brand Icon Controller
    const faviconToggleBtn = document.getElementById('faviconToggle');
    const faviconDropdown = document.getElementById('faviconDropdown');
    const brandLogoIcon = document.getElementById('brandLogoIcon');
    const currentFaviconDisplay = document.getElementById('currentFaviconDisplay');
    const faviconOptions = document.querySelectorAll('.favicon-option');

    const faviconSVGs = {
        lightning: `data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 24 24%22 fill=%22none%22 stroke=%22%236366f1%22 stroke-width=%222%22 stroke-linecap=%22round%22 stroke-linejoin=%22round%22><path d=%22M13 2L3 14h9l-1 8 10-12h-9l1-8z%22 fill=%22url(%23g)%22/><defs><linearGradient id=%22g%22 x1=%220%25%22 y1=%220%25%22 x2=%22100%25%22 y2=%22100%25%22><stop offset=%220%25%22 stop-color=%22%236366f1%22/><stop offset=%22100%25%22 stop-color=%22%23ec4899%22/></linearGradient></defs></svg>`,
        brain: `data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 24 24%22 fill=%22none%22 stroke=%22url(%23g)%22 stroke-width=%222%22 stroke-linecap=%22round%22 stroke-linejoin=%22round%22><defs><linearGradient id=%22g%22 x1=%220%25%22 y1=%220%25%22 x2=%22100%25%22 y2=%22100%25%22><stop offset=%220%25%22 stop-color=%22%23a855f7%22/><stop offset=%22100%25%22 stop-color=%22%23ec4899%22/></linearGradient></defs><path d=%22M9.5 2A2.5 2.5 0 0 1 12 4.5v15a2.5 2.5 0 0 1-4.96-.44 2.5 2.5 0 0 1-2-3.56 2.5 2.5 0 0 1 .04-3.5 2.5 2.5 0 0 1 2.5-4.5 2.5 2.5 0 0 1 1.92-3A2.5 2.5 0 0 1 9.5 2Z%22/><path d=%22M14.5 2A2.5 2.5 0 0 0 12 4.5v15a2.5 2.5 0 0 0 4.96-.44 2.5 2.5 0 0 0 2-3.56 2.5 2.5 0 0 0-.04-3.5 2.5 2.5 0 0 0-2.5-4.5 2.5 2.5 0 0 0-1.92-3A2.5 2.5 0 0 0 14.5 2Z%22/></svg>`,
        scholar: `data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 24 24%22 fill=%22none%22 stroke=%22url(%23g)%22 stroke-width=%222%22 stroke-linecap=%22round%22 stroke-linejoin=%22round%22><defs><linearGradient id=%22g%22 x1=%220%25%22 y1=%220%25%22 x2=%22100%25%22 y2=%22100%25%22><stop offset=%220%25%22 stop-color=%22%233b82f6%22/><stop offset=%22100%25%22 stop-color=%22%2310b981%22/></linearGradient></defs><path d=%22M21.42 10.922a1 1 0 0 0-.019-1.838L12.83 5.18a2 2 0 0 0-1.66 0L2.6 9.08a1 1 0 0 0 0 1.832l8.57 3.908a2 2 0 0 0 1.66 0z%22/><path d=%22M6 12v5c0 2 2 3 6 3s6-1 6-3v-5%22/><path d=%22M21.5 12v6%22/></svg>`,
        star: `data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 24 24%22 fill=%22url(%23g)%22 stroke=%22url(%23g)%22 stroke-width=%221%22 stroke-linejoin=%22round%22><defs><linearGradient id=%22g%22 x1=%220%25%22 y1=%220%25%22 x2=%22100%25%22 y2=%22100%25%22><stop offset=%220%25%22 stop-color=%22%23f59e0b%22/><stop offset=%22100%25%22 stop-color=%22%23f97316%22/></linearGradient></defs><polygon points=%2212 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2%22/></svg>`,
        target: `data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 24 24%22 fill=%22none%22 stroke=%22url(%23g)%22 stroke-width=%222%22 stroke-linecap=%22round%22 stroke-linejoin=%22round%22><defs><linearGradient id=%22g%22 x1=%220%25%22 y1=%220%25%22 x2=%22100%25%22 y2=%22100%25%22><stop offset=%220%25%22 stop-color=%22%23ef4444%22/><stop offset=%22100%25%22 stop-color=%22%23f43f5e%22/></linearGradient></defs><circle cx=%2212%22 cy=%2212%22 r=%2210%22/><circle cx=%2212%22 cy=%2212%22 r=%226%22/><circle cx=%2212%22 cy=%2212%22 r=%222%22/></svg>`
    };

    const emojiIcons = {
        lightning: '⚡',
        brain: '🧠',
        scholar: '🎓',
        star: '⭐',
        target: '🎯'
    };

    if (faviconToggleBtn && faviconDropdown) {
        // Load initial choice
        const activeChoice = localStorage.getItem('faviconChoice') || 'lightning';
        updateFaviconAndBrand(activeChoice);

        faviconToggleBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            faviconDropdown.classList.toggle('show');
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', () => {
            faviconDropdown.classList.remove('show');
        });

        // Option click handling
        faviconOptions.forEach(option => {
            option.addEventListener('click', () => {
                const choice = option.getAttribute('data-icon');
                updateFaviconAndBrand(choice);
                localStorage.setItem('faviconChoice', choice);
                faviconDropdown.classList.remove('show');
            });
        });
    }

    function updateFaviconAndBrand(choice) {
        // Update favicon in DOM
        let link = document.getElementById('faviconLink');
        if (!link) {
            link = document.createElement('link');
            link.id = 'faviconLink';
            link.rel = 'icon';
            link.type = 'image/svg+xml';
            document.head.appendChild(link);
        }
        link.href = faviconSVGs[choice] || faviconSVGs.lightning;

        // Update brand logo text
        const emoji = emojiIcons[choice] || '⚡';
        if (brandLogoIcon) {
            brandLogoIcon.textContent = emoji;
        }
        if (currentFaviconDisplay) {
            currentFaviconDisplay.textContent = emoji;
        }

        // Highlight active item
        faviconOptions.forEach(option => {
            if (option.getAttribute('data-icon') === choice) {
                option.classList.add('active');
            } else {
                option.classList.remove('active');
            }
        });
    }

    function updateToggleIcon(theme) {
        const moon = document.querySelector('.theme-toggle-btn .moon-icon');
        const sun = document.querySelector('.theme-toggle-btn .sun-icon');
        if (theme === 'dark') {
            if (moon) moon.style.opacity = '1';
            if (sun) sun.style.opacity = '0';
        } else {
            if (moon) moon.style.opacity = '0';
            if (sun) sun.style.opacity = '1';
        }
    }

    // 2. Interactive Focus & Hover Micro-interactions
    const formControls = document.querySelectorAll('.form-control');
    formControls.forEach(control => {
        control.addEventListener('focus', () => {
            control.parentElement.classList.add('focused');
        });
        control.addEventListener('blur', () => {
            control.parentElement.classList.remove('focused');
        });
    });

    // 3. Option Selection Radio helper (for Quiz Take option radios if any)
    const options = document.querySelectorAll('.option-label');
    options.forEach(option => {
        const radio = option.querySelector('input[type="radio"]');
        if (radio) {
            if (radio.checked) {
                option.classList.add('selected');
            }
            option.addEventListener('click', () => {
                options.forEach(o => o.classList.remove('selected'));
                option.classList.add('selected');
            });
        }
    });

    // 4. Subtle Card Float Animations
    const cards = document.querySelectorAll('.dashboard-card, .glass-card, .teacher-card');
    cards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.05}s`;
    });
});
