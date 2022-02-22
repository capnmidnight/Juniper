export function mapToObject<U>(map: Map<string, U>) {
    let obj: { [key: string]: U; } = {};
    for (const [key, value] of map) {
        obj[key] = value;
    }
    return obj;
}
