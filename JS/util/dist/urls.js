/**
 * Resolves a string or URL object to just a string
 */
export function unpackURL(value) {
    if (value instanceof URL) {
        value = value.href;
    }
    return value;
}
//# sourceMappingURL=urls.js.map