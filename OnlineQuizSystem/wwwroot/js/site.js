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
