'use strict';

// The startup cloak's phrase and its dismissal. Loaded synchronously directly after the cloak markup in
// index.html, so the phrase is swapped in before the parser moves on. The span already carries the first phrase
// as a literal, so a paint that lands before this runs still shows words rather than a bare slate - this only
// swaps in the random pick, and is why that one phrase appears in both places. Add, cut, or reword freely; the
// only thing each one has to do is read as "still working on it". MainLayout calls appCloak.dismiss when the
// layout has resolved.
(() => {
    const phrases = [
        'Stoking the forge...',
        'Gathering your materials...',
        'Hammering out the details...',
        'Sharpening the axe...',
        'Counting every last nail...',
        'Smelting down the numbers...',
        'Foraging for spare parts...',
        'Waking up the blacksmith...'
    ];

    const cloak = document.getElementById('app-cloak');
    cloak.firstElementChild.textContent = phrases[Math.floor(Math.random() * phrases.length)];

    globalThis.appCloak = {
        dismiss() {
            cloak.classList.add('app-cloak-fading');
            // On a timer rather than transitionend, which never fires when prefers-reduced-motion collapses the
            // duration to zero. Reading the duration back off the element keeps this from drifting out of step
            // with the stylesheet. The unit is parsed rather than assumed: Chromium serializes a computed <time>
            // in seconds whatever the stylesheet wrote, but an engine that answered "300ms" would hold the cloak
            // for 300 seconds.
            const duration = getComputedStyle(cloak).transitionDuration.trim();
            const ms = Number.parseFloat(duration) * (duration.endsWith('ms') ? 1 : 1000);
            setTimeout(() => cloak.remove(), ms);
        }
    };
})();
