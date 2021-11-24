export function mapUnderwrite(dest, key, value) {
    if (!dest.has(key)) {
        dest.set(key, value);
    }
    return dest;
}
