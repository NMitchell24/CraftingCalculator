'use strict';

// The on-screen keyboard policy, for every field in the app at once: Enter commits and closes the keyboard, a field
// the keyboard would cover scrolls into view, a picker keeps the keyboard up while its buttons are tapped and drops
// it when its list is scrolled, and while the keyboard is up <html> carries .keyboard-open (app.css hides the bottom
// bars off it).
(() => {
    const root = document.documentElement;

    function isSearchFocused() {
        const active = document.activeElement;
        return active !== null && active.tagName === 'INPUT' && active.closest('.record-search') !== null;
    }

    // MudTextField and MudNumericField commit on change, which the blur raises, so the bound value lands before the
    // keyboard closes. A textarea is not an INPUT and keeps Enter as a newline. MudSelect's input is excluded
    // because Enter opens and picks from its list on a hardware keyboard.
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && e.target.tagName === 'INPUT' && !e.target.closest('.mud-select')) {
            e.target.blur();
        }
    });

    // Labels the keyboard's Enter key to match. Set on focus rather than on every field's markup; the IME reads the
    // attribute when the field connects, which is after focusin. A field that already names its own hint
    // (RecordSearchBar's "search") keeps it.
    document.addEventListener('focusin', (e) => {
        if (e.target.tagName === 'INPUT' && !e.target.hasAttribute('enterkeyhint')) {
            e.target.setAttribute('enterkeyhint', 'done');
        }
    });

    // A tap moves focus on mousedown, so cancelling it keeps the search field focused and the keyboard up: search,
    // tap a picker card's stepper, Add or Delete, search again. Only while the search field has focus, because a
    // quantity field being typed into has to blur and commit before its own step applies.
    document.addEventListener('mousedown', (e) => {
        if (e.target.closest('.record-card-zone button') && isSearchFocused()) {
            e.preventDefault();
        }
    }, true);

    // Scrolling a picker's list means the user is done typing, so the keyboard gets out of the way. This is a user
    // gesture, not a resize: blurring on resize is what sends an Android WebView into a focus/keyboard loop.
    document.addEventListener('touchmove', (e) => {
        if (isSearchFocused() && e.target.closest('.picker-dialog-content') && !e.target.closest('.record-search')) {
            document.activeElement.blur();
        }
    }, { passive: true });

    // SafeAreaInsetsInjector keeps the IME inset away from the Android WebView, which also stops the WebView
    // scrolling a focused field clear of the keyboard, so this does it once the shrunk layout has landed. 'nearest'
    // leaves a field that is already in view where it is. WKWebView never shrinks innerHeight for the keyboard, so
    // on iOS this never runs and the WebView's own scroll stands.
    let lastHeight = globalThis.innerHeight;

    globalThis.addEventListener('resize', () => {
        const active = document.activeElement;

        if (globalThis.innerHeight < lastHeight && active !== null
            && (active.tagName === 'INPUT' || active.tagName === 'TEXTAREA')) {
            active.scrollIntoView({ block: 'nearest' });
        }

        lastHeight = globalThis.innerHeight;
    });

    // WKWebView leaves the layout viewport full height under the keyboard and shrinks only the visual viewport, so
    // the gap that leaves at the bottom is the keyboard. Android's SafeAreaInsetsInjector resizes the WebView itself
    // and marks the root with data-native-ime, where this would measure 0 and has to leave .keyboard-open alone. A
    // pinch zoom shrinks the visual viewport too, hence scale.
    const viewport = globalThis.visualViewport;

    if (viewport) {
        const onViewportChange = () => {
            if ('nativeIme' in root.dataset) {
                return;
            }

            const inset = Math.max(0, Math.round(globalThis.innerHeight - viewport.height - viewport.offsetTop));
            const open = inset > 0 && Math.abs(viewport.scale - 1) < 0.01;

            root.style.setProperty('--keyboard-inset', `${open ? inset : 0}px`);
            root.classList.toggle('keyboard-open', open);
        };

        viewport.addEventListener('resize', onViewportChange);
        viewport.addEventListener('scroll', onViewportChange);
    }
})();
