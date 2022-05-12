import { mapInvert } from "../collections/mapInvert";
import { assertNever } from "../typeChecks";

type Base = 2 | 10;

type Base2Units = "KiB"
    | "MiB"
    | "GiB"
    | "TiB";

type Base10Units = "KB"
    | "MB"
    | "GB"
    | "TB";

type Units = "B"
    | Base2Units
    | Base10Units;

function isBase2Units(label: Units): label is Base2Units {
    return label !== "B"
        && label[1] === 'i';
}

function isBase10Units(label: Units): label is Base10Units {
    return label !== "B"
        && !isBase10Units(label);
}

const base2Labels = new Map<number, Base2Units>([
    [1, "KiB"],
    [2, "MiB"],
    [3, "GiB"],
    [4, "TiB"]
]);

const base10Labels = new Map<number, Base10Units>([
    [1, "KB"],
    [2, "MB"],
    [3, "GB"],
    [4, "TB"]
]);

const base2Sizes = mapInvert(base2Labels);
const base10Sizes = mapInvert(base10Labels);

const labels = new Map<Base, Map<number, Units>>([
    [2, base2Labels],
    [10, base10Labels]
]);

export function formatBytes(value: number, base: 2 | 10 = 10) {
    const isNegative = value < 0;
    value = Math.abs(value);

    const systemBase = base === 2 ? 1024 : 1000;
    let size = Math.min(4, Math.floor(Math.log(value) / Math.log(systemBase)));
    let divisor = Math.pow(systemBase, size);
    if (2 * value >= systemBase * divisor && size < 4) {
        size++;
        divisor *= systemBase;
    }

    let label: string;
    if (size === 0) {
        label = "B";
    }
    else {
        const levels = labels.get(base);
        label = levels.get(size);
        value /= divisor;
    }
    const str = `${isNegative ? "-" : ""}${value.toFixed(2)} ${label}`;

    return str;
}

export function toBytes(value: number, units: Units): number {
    if (units === "B") {
        return value;
    }
    else {
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
        return value * multiplier;
    }
}