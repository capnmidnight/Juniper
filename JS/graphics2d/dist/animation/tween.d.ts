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
 | "QuadraticContinuous" | "Sine" | "SineContinuous"
/**
 * Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
 * the beginning and the end of the curve.
 **/
 | "Bump"
/**
 * Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
 * the beginning and the end of the curve.
 **/
 | "BumpContinuous" | "Jump";
/**
 * Implementations for each of the values in <see cref="TweenType"/>.
 * All functions receive a value t, representing the proportion of the way through the animation process we are
 * calculating. All functions take an extra `k` parameter, for scaling tertiary effects; most functions do not use it.
 * All functions return a value v, mapping the value t to one of the desired 'tween shapes.
 **/
export declare function isContinuous(tween: TweenType): boolean;
export declare function linear(t: number): number;
export declare function quadratic(t: number): number;
export declare function sine(t: number): number;
export declare function bump(t: number, k: number): number;
export declare function jump(t: number, k: number): number;
export declare function linearContinuous(t: number, _k: number, d: boolean): number;
export declare function quadraticContinuous(t: number, _k: number, d: boolean): number;
export declare function sineContinuous(t: number, _k: number, d: boolean): number;
export declare function bumpContinuous(t: number, k: number, d: boolean): number;
//# sourceMappingURL=tween.d.ts.map