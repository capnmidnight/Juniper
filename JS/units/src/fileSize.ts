import { makeReverseLookup, assertNever } from "@juniper-lib/util";

type Base = 2 | 10;

type Base2Units = "KiB"
    | "MiB"
    | "GiB"
    | "TiB"
    | "PiB";

type Base10Units = "KB"
    | "MB"
    | "GB"
    | "TB"
    | "PB";

type Units = "B"
    | Base2Units
    | Base10Units;

function isBase2Units(label: Units): label is Base2Units {
    return label !== "B"
        && label[1] === "i";
}

function isBase10Units(label: Units): label is Base10Units {
    return label !== "B"
        && !isBase10Units(label);
}

const base2Labels: Units[] = [
    "B",
    "KiB",
    "MiB",
    "GiB",
    "TiB",
    "PiB"
];

const base10Labels: Units[] = [
    "B",
    "KB",
    "MB",
    "GB",
    "TB",
    "PB"
];

const base2Sizes = /*@__PURE__*/ (function () { return makeReverseLookup(base2Labels); })();
const base10Sizes = /*@__PURE__*/ (function () { return makeReverseLookup(base10Labels); })();

const labels = /*@__PURE__*/ (function () {
    return new Map<Base, Units[]>([
        [2, base2Labels],
        [10, base10Labels]
    ]);
})();

/**
 * Converts a value in number of bytes to a value in base-2 or
 * base-10 memory units, e.g. KiB (kibibytes, 2^10) vs KB (kilobytes, bayse 10^3)
 * and adds the unit specifier to the string. The exact label that is used
 * is the largest label that results in the smallest, greater than 0
 * integer part of the number.
 * @param value the memory in bytes
 * @param units the unit of memory to convert to
 */
export function formatBytes(value: number, base: Base = 10): string {
    const isNegative = value < 0;
    value = Math.abs(value);

    const systemBase = base === 2 ? 1024 : 1000;
    let size = Math.min(4, Math.floor(Math.log(value) / Math.log(systemBase)));
    let divisor = Math.pow(systemBase, size);
    if (2 * value >= systemBase * divisor && size < 4) {
        size++;
        divisor *= systemBase;
    }

    const levels = labels.get(base);
    const label = levels[size];
    value /= divisor;

    const isExact = (value % 1) === 0;
    const str = `${isNegative ? "-" : ""}${value.toFixed(isExact ? 0 : 2)} ${label}`;

    return str;
}

/**
 * Converts a value in number of bytes to a value in base-2 or
 * base-10 memory units, e.g. KiB (kibibytes, 2^10) vs KB (kilobytes, bayse 10^3).
 * @param value the memory in bytes
 * @param units the unit of memory to convert to
 */
export function fromBytes(value: number, units: Units): number {
    const multiplier = getMultiplier(units);
    return value / multiplier;
}

/**
 * Converts a value in a number of bytes expressed as a base-2 or
 * base-10 memory units (e.g. KiB (kibibytes, 2^10) vs KB (kilobytes, bayse 10^3)) 
 * to a number of bytes.
 * @param value the value in base-2 or base-10 memory units
 * @param units the units to convert from
 * @returns 
 */
export function toBytes(value: number, units: Units): number {
    const multiplier = getMultiplier(units);
    return value * multiplier;
}

function getMultiplier(units: Units): number {
    if (units === "B") {
        return 1;
    }

    let systemBase: number;
    let size: number;
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
    return multiplier;
}
