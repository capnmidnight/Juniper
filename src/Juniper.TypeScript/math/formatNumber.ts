import { isNumber } from "@juniper/tslib";

export function formatNumber(value: number, digits = 0): string {
    if (isNumber(value)) {
        return value.toFixed(digits);
    }
    else {
        return "";
    }
}

export function parseNumber(value: string): number {
    if (/\d+/.test(value)) {
        return parseFloat(value);
    }
    else {
        return null;
    }
}
