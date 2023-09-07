export function mapInvert(map) {
    const mapOut = new Map();
    for (const [key, value] of map) {
        mapOut.set(value, key);
    }
    return mapOut;
}
//# sourceMappingURL=mapInvert.js.map