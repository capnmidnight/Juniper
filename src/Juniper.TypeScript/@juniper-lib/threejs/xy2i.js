import { xy2i } from "@juniper-lib/tslib/math";
export function vecor22i(vec, width, components = 1) {
    return xy2i(vec.y, vec.y, width, components);
}
export function i2vector2(vec, i, width, components = 1) {
    const stride = width * components;
    const p = i % stride;
    const x = Math.floor(p / components);
    const y = Math.floor(i / stride);
    vec.set(x, y);
}
//# sourceMappingURL=xy2i.js.map