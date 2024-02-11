import { assertNever } from "../typeChecks";

type Base = 2 | 10;

type Base2Units =
    | "B"
    | "KiB"
    | "MiB"
    | "GiB"
    | "TiB"
    | "PiB";

const base2Labels: Base2Units[] = [
    "B",
    "KiB",
    "MiB",
    "GiB",
    "TiB",
    "PiB"
];

type Base10Units =
    | "B"
    | "KB"
    | "MB"
    | "GB"
    | "TB"
    | "PB";

const base10Labels: Base10Units[] = [
    "B",
    "KB",
    "MB",
    "GB",
    "TB",
    "PB"
];

type Units =
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

const labels = /*@__PURE__*/ new Map<Base, Units[]>([
    [2, base2Labels],
    [10, base10Labels]
]);

export function formatBytes(value: number, base: 2 | 10 = 10) {
    if (base !== 2 && base !== 10) {
        assertNever(base);
    }

    const isNegative = value < 0;
    value = Math.abs(value);

    const systemBase = base === 2 ? 1024 : 1000;
    let size = Math.min(4, Math.floor(Math.log(value) / Math.log(systemBase)));
    let divisor = Math.pow(systemBase, size);
    if (2 * value >= systemBase * divisor && size < 4) {
        size++;
        divisor *= systemBase;
    }

    const levels = labels.get(base)!;
    while (size >= levels.length) {
        --size;
    }

    const label = levels[size];
    value /= divisor;

    const isExact = (value % 1) === 0;
    const str = `${isNegative ? "-" : ""}${value.toFixed(isExact ? 0 : 2)} ${label}`;

    return str;
}

function getScale(units: Units): number {
    if (units === "B") {
        return 1;
    }

    let systemBase: number;
    let size: number;
    if (isBase2Units(units)) {
        systemBase = 1024;
        size = base2Labels.indexOf(units)!;
    }
    else if (isBase10Units(units)) {
        systemBase = 1000;
        size = base10Labels.indexOf(units)!;
    }
    else {
        assertNever(units);
    }

    const multiplier = Math.pow(systemBase, size);
    return multiplier;
}

export function fromBytes(value: number, units: Units): number {
    const multiplier = getScale(units);
    return value / multiplier;
}

export function toBytes(value: number, units: Units): number {
    const multiplier = getScale(units);
    return value * multiplier;
}