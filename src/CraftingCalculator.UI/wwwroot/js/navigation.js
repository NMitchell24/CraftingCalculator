'use strict';

// Where a navigation lands: the history entry the app is on (appHistory), the scroll offset each entry is shown at
// (appScroll), and the heading a help link names (helpScroll).

// Read by MainLayout.OpenDestinationAsync and by appScroll below. Blazor's NavigationManager stamps every history
// entry it creates with _index, the entry's position in the session history; the entry the app starts on carries
// no state, so it is 0. Read from here rather than counted in .NET, where LocationChanged cannot tell a step back
// from a new entry, and rather than history.length, which also counts the entries ahead of the current one.
globalThis.appHistory = {
    index() {
        return history.state?._index ?? 0;
    }
};

// MainLayout calls appScroll.restore once the router has rendered the new page, and Import calls appScroll.toTop
// when its wizard changes step.
//
// Blazor's WebView navigation arms a window.scrollTo(0,0) and fires it on the next render batch. That is usually
// the no-op batch Blazor renders after the click handler, so the OUTGOING page snaps to the top before the router
// swaps content, which is the flash on tapping Edit far down a list. It also does nothing on back, so returning to
// a long list lands at the top. This module replaces it and positions the document itself before the paint, in
// both directions.
//
// Offsets are keyed by the history entry index Blazor stamps into history.state (appHistory.index), which gives
// browser semantics: back returns to an index that already has a saved offset, while a fresh navigation pushes an
// index whose forward branch has been discarded and so starts at the top. A replace (Help's pages, the bottom nav)
// keeps the index but changes the page, and starts at the top too.
globalThis.appScroll = (() => {
    const positions = new Map();
    // Frames the settle loop keeps re-applying the offset for while the page is still growing.
    const settleFrames = 40;
    // Consecutive frames of unchanged page height that count as "this page is done growing".
    const stableFramesToSettle = 3;

    const scroller = document.scrollingElement ?? document.documentElement;
    // Left on "auto", the browser also restores the popped entry's offset on popstate, against the OUTGOING page,
    // which is still on screen and usually a different length.
    history.scrollRestoration = 'manual';

    // The only place this reaches into Blazor's internals, and load-bearing: correcting the offset afterwards
    // cannot remove the flash, because the reset has already been painted. Blazor exposes no way to turn it off,
    // so the call is intercepted. Matched on the exact shape Blazor uses, two positional zeroes; MudBlazor passes
    // an options object, and toTop below goes around it. The call lives in the embedded blazor.webview.js as
    // window.scrollTo&&window.scrollTo(0,0). If a future .NET stops routing its reset through window.scrollTo,
    // this stops matching and the flash returns.
    const nativeScrollTo = globalThis.scrollTo.bind(globalThis);
    globalThis.scrollTo = (...args) => {
        if (args.length === 2 && args[0] === 0 && args[1] === 0) {
            return;
        }

        nativeScrollTo(...args);
    };

    let savePending = false;
    // The history entry whose content the page is showing and scrolling. The page is the URL without its
    // fragment, so a replace that swaps the page is told apart from one that only moves an anchor.
    let settledIndex = appHistory.index();
    let settledPage = currentPage();
    // Non-null while a navigation is in flight: the destination index and the offset it should land at.
    let armedIndex = null;
    let armedTarget = 0;
    let settleToken = 0;
    // Whether the pending entry change came from a back or forward step rather than a push.
    let popped = false;

    globalThis.addEventListener('popstate', () => {
        popped = true;
    });

    globalThis.addEventListener('scroll', onScroll, { passive: true });

    // Blazor swaps the page's nodes in one batch; this callback is a microtask, so it runs after that batch but
    // before the browser paints it, which keeps the new page from appearing at the wrong offset for a frame and
    // then jumping.
    new MutationObserver(() => {
        if (onSettledEntry()) {
            // A back that a location-changing handler stopped (DatasetEditor's "Discard changes?") leaves Blazor
            // stepping away and straight back with history.go, and a render in between arms the entry it stepped
            // to. No LocationChanged follows to settle it, and an armed module would pin the page to that offset
            // on every later render.
            if (armedIndex !== null) {
                disarm();
            }

            return;
        }

        apply();
    }).observe(document.getElementById('app'), { childList: true, subtree: true });

    function currentPage() {
        return location.origin + location.pathname + location.search;
    }

    function onSettledEntry() {
        const index = appHistory.index();

        if (index === settledIndex && currentPage() === settledPage) {
            return true;
        }

        // A link to a heading on the page already showing (calculations.md#surplus, from calculations.md) is
        // handled by Blazor in script: it pushes an entry and scrolls to the heading, with no render and no
        // LocationChanged, so restore never runs for it. Adopted here without moving the page.
        if (!popped && armedIndex === null && currentPage() === settledPage) {
            settledIndex = index;
            return true;
        }

        return false;
    }

    function onScroll() {
        if (savePending || armedIndex !== null || !onSettledEntry()) {
            return;
        }

        // Coalesce the scroll event stream down to one write per frame.
        savePending = true;
        requestAnimationFrame(() => {
            savePending = false;

            if (armedIndex === null && onSettledEntry()) {
                positions.set(settledIndex, scroller.scrollTop);
            }
        });
    }

    function arm(index) {
        if (armedIndex === index) {
            return;
        }

        // A push discards the whole forward branch, and Blazor reuses those indices, so their saved offsets would
        // otherwise be handed to unrelated pages. A replace discards the entry it lands on. Only a pop reaches a
        // live entry.
        if (!popped) {
            for (const key of positions.keys()) {
                if (key >= index) {
                    positions.delete(key);
                }
            }
        }

        popped = false;
        armedIndex = index;
        armedTarget = positions.get(index) ?? 0;
    }

    function apply() {
        arm(appHistory.index());
        // Assigning past the page's end clamps, which is the right answer whenever the destination is genuinely
        // shorter than it was: a record deleted in the editor, say.
        scroller.scrollTop = armedTarget;
    }

    function disarm() {
        armedIndex = null;
        popped = false;
        settleToken++;
        scroller.scrollTop = positions.get(settledIndex) ?? 0;
    }

    return {
        // The observer above has usually already positioned the page; this covers a navigation that mutated
        // nothing, and re-applies the offset across any frames the page spends still growing.
        restore() {
            apply();

            const index = armedIndex;
            const target = armedTarget;
            const token = ++settleToken;
            let frames = 0;
            let stableFrames = 0;
            let lastHeight = -1;

            const settle = () => {
                if (token !== settleToken) {
                    return;
                }

                const height = scroller.scrollHeight;
                stableFrames = height === lastHeight ? stableFrames + 1 : 0;
                lastHeight = height;

                scroller.scrollTop = target;
                frames++;

                // Settle once the offset is reached, once the page has stopped growing and so can never reach it
                // (the clamp is then the final answer), or on the frame budget.
                if (scroller.scrollTop === target || stableFrames >= stableFramesToSettle || frames > settleFrames) {
                    settledIndex = index;
                    settledPage = currentPage();
                    armedIndex = null;
                    return;
                }

                requestAnimationFrame(settle);
            };

            settle();
        },

        // For content that replaces itself without navigating.
        toTop() {
            nativeScrollTo(0, 0);
        }
    };
})();

// Called by Help.razor after an article opened at a heading renders: a cross-page anchor (calculations.md#surplus)
// has to reach its heading rather than the top of the page.
globalThis.helpScroll = {
    // Headings take their ids from Markdig's UseAutoIdentifiers, so a link to one that has since been renamed
    // resolves to nothing, and the page stays at the top appScroll put it at. Where a found heading stops is
    // scroll-margin-top on .help-article's headings in app.css, which clears the fixed app bar.
    toTarget(id) {
        document.getElementById(id)?.scrollIntoView();
    }
};
