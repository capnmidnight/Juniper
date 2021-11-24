import { isNumber } from "../";
export function formatNumber(value, digits = 0) {
    if (isNumber(value)) {
        return value.toFixed(digits);
    }
    else {
        return "";
    }
}
export function parseNumber(value) {
    if (/\d+/.test(value)) {
        return parseFloat(value);
    }
    else {
        return null;
    }
}
