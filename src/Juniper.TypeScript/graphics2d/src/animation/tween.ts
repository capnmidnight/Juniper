export type TweenFunc = (t: number, k: number, d: boolean) => number;

/**
 * Different modes of animating a single value from 0 to 1.
 **/
export type TweenType =
    /**
     * The most basic form of `tween, values increase in proportion to time, from
     * 0 to 1 in the forward direction, then 1 to 0 in the reverse direction.
     **/
    "Linear"

    /**
     * Values increase in proportion to time from -1 to 0 in the forward direction,
     * then 0 to 1 in the reverse, rather than decreasing from 1 to 0.
     **/
    | "LinearContinuous"

    /**
     * Values increase in proportion to the square of time, from 0 to 1 in the forward direction,
     * then 1 to 0 in the reverse direction.
     **/
    | "Quadratic"

    /**
     * Values increase in proportion to the square of time. from -1 to 0 in the forward direction,
     * then 0 to 1 in the reverse direction.
     **/
    | "QuadraticContinuous"

    | "Sine"

    | "SineContinuous"

    /**
     * Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
     * the beginning and the end of the curve.
     **/
    | "Bump"

    /**
     * Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
     * the beginning and the end of the curve.
     **/
    | "BumpContinuous"

    | "Jump";

/**
 * Implementations for each of the values in <see cref="TweenType"/>.
 * All functions receive a value t, representing the proportion of the way through the animation process we are
 * calculating. All functions take an extra `k` parameter, for scaling tertiary effects; most functions do not use it.
 * All functions return a value v, mapping the value t to one of the desired 'tween shapes.
 **/
export function isContinuous(tween: TweenType): boolean {
    return tween == "LinearContinuous"
        || tween == "QuadraticContinuous"
        || tween == "SineContinuous"
        || tween == "BumpContinuous";
}

export function linear(t: number): number {
    return t;
}

export function quadratic(t: number): number {
    return t * t;
}

export function sine(t: number): number {
    var a = t * Math.PI;
    return Math.sin(a);
}

export function bump(t: number, k: number): number {
    var a = t * Math.PI;
    return ((0.5 * (1 - Math.cos(a))) - (k * Math.sin(2 * a)));
}

export function jump(t: number, k: number): number {
    var a = (t - 0.5) * Math.PI;
    return t * t + k * Math.cos(a);
}

export function linearContinuous(t: number, _k: number, d: boolean): number {
    if (d) {
        return t - 1;
    }
    else {
        return 1 - t;
    }
}

export function quadraticContinuous(t: number, _k: number, d: boolean): number {
    t = linearContinuous(t, null, d);
    return Math.sign(t) * quadratic(t);
}

export function sineContinuous(t: number, _k: number, d: boolean): number {
    t = linearContinuous(t, null, d);
    return sine(t);
}

export function bumpContinuous(t: number, k: number, d: boolean): number {
    t = linearContinuous(t, null, d);
    return bump(t, k);
}