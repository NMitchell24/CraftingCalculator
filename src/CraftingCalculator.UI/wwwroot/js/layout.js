'use strict';

// Publishes the measured border-box height of every element that carries a data-height-var attribute as a custom
// property on the root element, named by that attribute's value, so the stylesheet can offset by a bar whose height
// depends on its content: the app bar grows when its title wraps (MainLayout tags it, and --app-bar-bottom in
// app.css reads the result). The property is removed when the element leaves the DOM, which lets the stylesheet's
// fallback value stand in until the next one renders.
(() => {
    const root = document.documentElement;
    const tracked = new Map();

    const resizeObserver = new ResizeObserver((entries) => {
        for (const entry of entries) {
            const name = tracked.get(entry.target);

            if (name) {
                root.style.setProperty(name, `${entry.target.getBoundingClientRect().height}px`);
            }
        }
    });

    // Blazor renders and removes elements as pages change, so the set is re-read on every DOM mutation rather than
    // once; the attribute selector keeps that cheap.
    function sync() {
        const current = new Set(document.querySelectorAll('[data-height-var]'));

        for (const [element, name] of tracked) {
            if (!current.has(element)) {
                resizeObserver.unobserve(element);
                tracked.delete(element);
                root.style.removeProperty(name);
            }
        }

        for (const element of current) {
            if (!tracked.has(element)) {
                tracked.set(element, element.dataset.heightVar);
                resizeObserver.observe(element);
            }
        }
    }

    new MutationObserver(sync).observe(document.body, { childList: true, subtree: true });
    sync();
})();
