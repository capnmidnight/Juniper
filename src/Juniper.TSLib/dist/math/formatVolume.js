import { isNumber } from "../typeChecks";
import { clamp } from "./clamp";
import { project } from "./project";
import { unproject } from "./unproject";
export function formatVolume(value) {
    if (isNumber(value)) {
        return clamp(unproject(value, 0, 100), 0, 100).toFixed(0);
    }
    else {
        return "";
    }
}
export function parseVolume(value) {
    if (/\d+/.test(value)) {
        return clamp(project(parseInt(value, 10), 0, 100), 0, 1);
    }
    else {
        return null;
    }
}
