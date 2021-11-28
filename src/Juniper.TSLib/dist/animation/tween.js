/**
 * Different modes of animating a single value from 0 to 1.
 **/
export var TweenType;
(function (TweenType) {
    /**
     * The most basic form of `tween, values increase in proportion to time, from
     * 0 to 1 in the forward direction, then 1 to 0 in the reverse direction.
     **/
    TweenType[TweenType["Linear"] = 0] = "Linear";
    /**
     * Values increase in proportion to time from -1 to 0 in the forward direction,
     * then 0 to 1 in the reverse, rather than decreasing from 1 to 0.
     **/
    TweenType[TweenType["LinearContinuous"] = 1] = "LinearContinuous";
    /**
     * Values increase in proportion to the square of time, from 0 to 1 in the forward direction,
     * then 1 to 0 in the reverse direction.
     **/
    TweenType[TweenType["Quadratic"] = 2] = "Quadratic";
    /**
     * Values increase in proportion to the square of time. from -1 to 0 in the forward direction,
     * then 0 to 1 in the reverse direction.
     **/
    TweenType[TweenType["QuadraticContinuous"] = 3] = "QuadraticContinuous";
    TweenType[TweenType["Sine"] = 4] = "Sine";
    TweenType[TweenType["SineContinuous"] = 5] = "SineContinuous";
    /**
     * Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
     * the beginning and the end of the curve.
     **/
    TweenType[TweenType["Bump"] = 6] = "Bump";
    /**
     * Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
     * the beginning and the end of the curve.
     **/
    TweenType[TweenType["BumpContinuous"] = 7] = "BumpContinuous";
    TweenType[TweenType["Jump"] = 8] = "Jump";
})(TweenType || (TweenType = {}));
/**
 * Implementations for each of the values in <see cref="TweenType"/>.
 * All functions receive a value t, representing the proportion of the way through the animation process we are
 * calculating. All functions take an extra `k` parameter, for scaling tertiary effects; most functions do not use it.
 * All functions return a value v, mapping the value t to one of the desired 'tween shapes.
 **/
export function isContinuous(tween) {
    return tween == TweenType.LinearContinuous
        || tween == TweenType.QuadraticContinuous
        || tween == TweenType.SineContinuous
        || tween == TweenType.BumpContinuous;
}
/**
 * A lookup for the tween functions, so we don't have to use reflection every time we want one.
 **/
export const functions = /*@__PURE__*/ new Map([
    [TweenType.Linear, linear],
    [TweenType.LinearContinuous, linearContinuous],
    [TweenType.Quadratic, quadratic],
    [TweenType.QuadraticContinuous, quadraticContinuous],
    [TweenType.Sine, sine],
    [TweenType.SineContinuous, sineContinuous],
    [TweenType.Bump, bump],
    [TweenType.BumpContinuous, bumpContinuous],
    [TweenType.Jump, jump]
]);
export function linear(t) {
    return t;
}
export function quadratic(t) {
    return t * t;
}
export function sine(t) {
    var a = t * Math.PI;
    return Math.sin(a);
}
export function bump(t, k) {
    var a = t * Math.PI;
    return ((0.5 * (1 - Math.cos(a))) - (k * Math.sin(2 * a)));
}
export function jump(t, k) {
    var a = (t - 0.5) * Math.PI;
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
