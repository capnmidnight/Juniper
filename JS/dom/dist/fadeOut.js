export function fadeOut(element, opaqueSeconds = 1, fadeSeconds = 1, dt = 0.01) {
    opaqueSeconds *= 1000;
    fadeSeconds *= 1000;
    dt *= 1000;
    let time = opaqueSeconds + fadeSeconds;
    const timer = setInterval(() => {
        time -= dt;
        const opacity = time > fadeSeconds ? 1 : (time / fadeSeconds);
        element.style.opacity = `${Math.min(100, opacity * 100)}%`;
        if (time === 0) {
            element.style.display = "none";
            clearInterval(timer);
        }
    }, dt);
}
//# sourceMappingURL=fadeOut.js.map