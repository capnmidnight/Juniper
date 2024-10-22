import { Pi } from "@juniper-lib/tslib/math";
/**
 * Implementations for each of the values in <see cref="TweenType"/>.
 * All functions receive a value t, representing the proportion of the way through the animation process we are
 * calculating. All functions take an extra `k` parameter, for scaling tertiary effects; most functions do not use it.
 * All functions return a value v, mapping the value t to one of the desired 'tween shapes.
 **/
export function isContinuous(tween) {
    return tween == "LinearContinuous"
        || tween == "QuadraticContinuous"
        || tween == "SineContinuous"
        || tween == "BumpContinuous";
}
export function linear(t) {
    return t;
}
export function quadratic(t) {
    return t * t;
}
export function sine(t) {
    const a = t * Pi;
    return Math.sin(a);
}
export function bump(t, k) {
    const a = t * Pi;
    return ((0.5 * (1 - Math.cos(a))) - (k * Math.sin(2 * a)));
}
export function jump(t, k) {
    const a = (t - 0.5) * Pi;
    return t * t + k * Math.cos(a);
}
export function linearContinuous(t, _k, d) {
    if (d) {
        return t - 1;
    }
    else {
        return 1 - t;
    }
}
export function quadraticContinuous(t, _k, d) {
    t = linearContinuous(t, null, d);
    return Math.sign(t) * quadratic(t);
}
export function sineContinuous(t, _k, d) {
    t = linearContinuous(t, null, d);
    return sine(t);
}
export function bumpContinuous(t, k, d) {
    t = linearContinuous(t, null, d);
    return bump(t, k);
}
//# sourceMappingURL=tween.js.map