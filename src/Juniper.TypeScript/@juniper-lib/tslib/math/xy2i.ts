import { vec2 } from "gl-matrix";

export function xy2i(x: number, y: number, width: number, components: number = 1): number {
    return components * (x + width * y);
}

export function vec22i(vec: vec2, width: number, components: number = 1): number {
    return xy2i(vec[0], vec[1], width, components);
}

export function i2vec2(vec: vec2, i: number, width: number, components: number = 1): void {
    const stride = width * components;
    const p = i % stride;
    const x = Math.floor(p / components);
    const y = Math.floor(i / stride);
    vec2.set(vec, x, y);
}