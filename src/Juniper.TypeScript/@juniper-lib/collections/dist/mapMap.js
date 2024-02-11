export function mapMap(items, makeID, makeValue) {
    return new Map(items.map((item, i) => [
        makeID(item, i),
        makeValue(item, i)
    ]));
}
//# sourceMappingURL=mapMap.js.map