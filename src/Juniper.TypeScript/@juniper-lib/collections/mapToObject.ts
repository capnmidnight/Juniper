export function mapToObject<U>(map: Map<string, U>) {
    const obj: { [key: string]: U; } = {};
    for (const [key, value] of map) {
        obj[key] = value;
    }
    return obj;
}
