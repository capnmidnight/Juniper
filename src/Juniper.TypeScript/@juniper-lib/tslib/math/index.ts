import { vec3 } from "gl-matrix";

export * from "./angleClamp";
export * from "./clamp";
export * from "./deg2rad";
export * from "./formatNumber";
export * from "./formatVolume";
export * from "./lerp";
export * from "./Point";
export * from "./powerOf2";
export * from "./project";
export * from "./rad2deg";
export * from "./Rectangle";
export * from "./Size";
export * from "./truncate";
export * from "./unproject";
export * from "./warnOnNaN";
export * from "./xy2i";

export const RIGHT = /*@__PURE__*/ vec3.fromValues(1, 0, 0);
export const UP = /*@__PURE__*/ vec3.fromValues(0, 1, 0);
export const FWD = /*@__PURE__*/ vec3.fromValues(0, 0, -1);