import { objectResolve, Objects } from "./objects";

export const FOREGROUND = 0;
export const LEFT_EYE = 1;
export const RIGHT_EYE = 2;
export const PURGATORY = 3;
export const PHOTOSPHERE_CAPTURE = 31;

export function deepSetLayer(obj: Objects, level: number) {
    obj = objectResolve(obj);
    obj.traverse(o => o.layers.set(level));
}

export function deepEnableLayer(obj: Objects, level: number) {
    obj = objectResolve(obj);
    obj.traverse(o => o.layers.enable(level));
}