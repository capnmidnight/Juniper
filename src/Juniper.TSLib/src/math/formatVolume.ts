import { clamp, isNumber, project, unproject } from "../";

export function formatVolume(value: number): string {
    if (isNumber(value)) {
        return clamp(unproject(value, 0, 100), 0, 100).toFixed(0);
    }
    else {
        return "";
    }
}

export function parseVolume(value: string): number {
    if (/\d+/.test(value)) {
        return clamp(project(parseInt(value, 10), 0, 100), 0, 1);
    }
    else {
        return null;
    }
}
