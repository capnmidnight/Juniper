/**
 * Resolves a string or URL object to just a string
 */
export function unpackURL(value: string | URL): string {
    if (value instanceof URL) {
        value = value.href;
    }

    return value;
}
