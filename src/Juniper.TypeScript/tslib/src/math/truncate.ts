export function truncate(v: number): number {
    if (Math.abs(v) > 0.0001) {
        return v;
    }

    return 0;
}
