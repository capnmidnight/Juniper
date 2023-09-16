import { assertNever } from "../typeChecks";
function mapInvert(map) {
    const mapOut = new Map();
    for (const [key, value] of map) {
        mapOut.set(value, key);
    }
    return mapOut;
}
function isBase2Units(label) {
    return label !== "B"
        && label[1] === "i";
}
function isBase10Units(label) {
    return label !== "B"
        && !isBase10Units(label);
}
const base2Labels = new Map([
    [1, "KiB"],
    [2, "MiB"],
    [3, "GiB"],
    [4, "TiB"]
]);
const base10Labels = new Map([
    [1, "KB"],
    [2, "MB"],
    [3, "GB"],
    [4, "TB"]
]);
const base2Sizes = /*@__PURE__*/ mapInvert(base2Labels);
const base10Sizes = /*@__PURE__*/ mapInvert(base10Labels);
const labels = /*@__PURE__*/ new Map([
    [2, base2Labels],
    [10, base10Labels]
]);
export function formatBytes(value, base = 10) {
    const isNegative = value < 0;
    value = Math.abs(value);
    const systemBase = base === 2 ? 1024 : 1000;
    let size = Math.min(4, Math.floor(Math.log(value) / Math.log(systemBase)));
    let divisor = Math.pow(systemBase, size);
    if (2 * value >= systemBase * divisor && size < 4) {
        size++;
        divisor *= systemBase;
    }
    let label;
    if (size === 0) {
        label = "B";
    }
    else {
        const levels = labels.get(base);
        label = levels.get(size);
        value /= divisor;
    }
    const isExact = (value % 1) === 0;
    const str = `${isNegative ? "-" : ""}${value.toFixed(isExact ? 0 : 2)} ${label}`;
    return str;
}
export function toBytes(value, units) {
    if (units === "B") {
        return value;
    }
    else {
        let systemBase;
        let size;
        if (isBase2Units(units)) {
            systemBase = 1024;
            size = base2Sizes.get(units);
        }
        else if (isBase10Units(units)) {
            systemBase = 1000;
            size = base10Sizes.get(units);
        }
        else {
            assertNever(units);
        }
        const multiplier = Math.pow(systemBase, size);
        return value * multiplier;
    }
}
//# sourceMappingURL=fileSize.js.map