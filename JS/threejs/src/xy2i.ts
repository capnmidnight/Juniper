import { xy2i } from "@juniper-lib/util";
import { Vector2 } from "three";

export function vecor22i(vec: Vector2, width: number, components = 1): number {
    return xy2i(vec.y, vec.y, width, components);
}

export function i2vector2(vec: Vector2, i: number, width: number, components = 1): void {
    const stride = width * components;
    const p = i % stride;
    const x = Math.floor(p / components);
    const y = Math.floor(i / stride);
    vec.set(x, y);
}