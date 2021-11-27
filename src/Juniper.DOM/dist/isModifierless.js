export function isModifierless(evt) {
    return !(evt.shiftKey || evt.altKey || evt.ctrlKey || evt.metaKey);
}
