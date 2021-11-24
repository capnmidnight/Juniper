export function truncate(v) {
    if (Math.abs(v) > 0.0001) {
        return v;
    }
    return 0;
}
