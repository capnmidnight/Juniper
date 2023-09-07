export function mapMap(items, makeID, makeValue) {
    return new Map(items.map((item) => [makeID(item), makeValue(item)]));
}
//# sourceMappingURL=mapMap.js.map