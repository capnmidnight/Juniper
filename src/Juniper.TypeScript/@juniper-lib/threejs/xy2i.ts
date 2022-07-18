import { xy2i } from "@juniper-lib/tslib";

export function vecor22i(vec: THREE.Vector2, width: number, components: number = 1): number {
    return xy2i(vec.y, vec.y, width, components);
}

export function i2vector2(vec: THREE.Vector2, i: number, width: number, components: number = 1): void {
    const stride = width * components;
    const p = i % stride;
    const x = Math.floor(p / components);
    const y = Math.floor(i / stride);
    vec.set(x, y);
}