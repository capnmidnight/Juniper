import { objectResolve, Objects } from "./objects";

export const FOREGROUND = /*@__PURE__*/ 0;
export const LEFT_EYE = /*@__PURE__*/ 1;
export const RIGHT_EYE = /*@__PURE__*/ 2;
export const PURGATORY = /*@__PURE__*/ 3;
export const PHOTOSPHERE_CAPTURE = /*@__PURE__*/ 31;

export function deepSetLayer(obj: Objects, level: number) {
    obj = objectResolve(obj);
    obj.traverse((o) => o.layers.set(level));
}

export function deepEnableLayer(obj: Objects, level: number) {
    obj = objectResolve(obj);
    obj.traverse((o) => o.layers.enable(level));
}

