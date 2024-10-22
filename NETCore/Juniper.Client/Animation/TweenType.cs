using Juniper.Input;

using System;
using System.Collections.Generic;

namespace Juniper.Animation
{
    public delegate float TweenFunc(float t, float k, Direction d);

    /// <summary>
    /// Different modes of animating a single value from 0 to 1.
    /// </summary>
    public enum TweenType
    {
        /// <summary>
        /// The most basic form of `tween, values increase in proportion to time, from
        /// 0 to 1 in the forward direction, then 1 to 0 in the reverse direction.
        /// </summary>
        Linear,

        /// <summary>
        /// Values increase in proportion to time from -1 to 0 in the forward direction,
        /// then 0 to 1 in the reverse, rather than decreasing from 1 to 0.
        /// </summary>
        LinearContinuous,

        /// <summary>
        /// Values increase in proportion to the square of time, from 0 to 1 in the forward direction,
        /// then 1 to 0 in the reverse direction.
        /// </summary>
        Quadratic,

        /// <summary>
        /// Values increase in proportion to the square of time. from -1 to 0 in the forward direction,
        /// then 0 to 1 in the reverse direction.
        /// </summary>
        QuadraticContinuous,

        Sine,

        SineContinuous,

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        Bump,

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        BumpContinuous
    }

    /// <summary>
    /// Implementations for each of the values in <see cref="TweenType"/>.
    /// All functions receive a value t, representing the proportion of the way through the animation process we are
    /// calculating. All functions take an extra `k` parameter, for scaling tertiary effects; most functions do not use it.
    /// All functions return a value v, mapping the value t to one of the desired 'tween shapes.
    /// </summary>
    public static class Tween
    {
        public static bool IsContinuous(TweenType tween)
        {
            return tween == TweenType.LinearContinuous
                || tween == TweenType.QuadraticContinuous
                || tween == TweenType.SineContinuous
                || tween == TweenType.BumpContinuous;
        }

        /// <summary>
        /// A lookup for the tween functions, so we don't have to use reflection every time we want one.
        /// </summary>
        public static readonly Dictionary<TweenType, TweenFunc> Functions = new Dictionary<TweenType, TweenFunc>(6)
        {
            { TweenType.Linear, Linear },
            { TweenType.LinearContinuous, LinearContinuous },
            { TweenType.Quadratic, Quadratic },
            { TweenType.QuadraticContinuous, QuadraticContinuous },
            { TweenType.Sine, Sine },
            { TweenType.SineContinuous, SineContinuous },
            { TweenType.Bump, Bump },
            { TweenType.BumpContinuous, BumpContinuous }
        };

        /// <summary>
        /// The most basic form of `tween, values increase in proportion to time.
        /// </summary>
        /// <param name="t">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Linear(float t, float k, Direction d)
        {
            return t;
        }

        /// <summary>
        /// The most basic form of `tween, values increase in proportion to time.
        /// </summary>
        /// <param name="t">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float LinearContinuous(float t, float k, Direction d)
        {
            if (d == Direction.Forward)
            {
                return t - 1;
            }
            else
            {
                return 1 - t;
            }
        }

        /// <summary>
        /// Values increase in proportion to the square of time, meaning they start growing
        /// slowly and then grow quickly.
        /// </summary>
        /// <param name="t">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Quadratic(float t, float k, Direction d)
        {
            return t * t;
        }

        /// <summary>
        /// Values increase in proportion to the square of time.
        /// </summary>
        /// <param name="t">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float QuadraticContinuous(float t, float k, Direction d)
        {
            t = LinearContinuous(t, k, d);
            return Math.Sign(t) * Quadratic(t, k, d);
        }

        /// <summary>
        /// Values increase on a sine curve, meaning they start growing quickly and
        /// then slow down.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="k"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static float Sine(float t, float k, Direction d)
        {
            var a = t * Math.PI;
            return (float)Math.Sin(a);
        }


        /// <summary>
        /// Values increase on a sine curve, meaning they start growing quickly and
        /// then slow down.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="k"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static float SineContinuous(float t, float k, Direction d)
        {
            var a = LinearContinuous(t, k, d) * Math.PI;
            return (float)Math.Sin(a);
        }

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        /// <param name="t">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Bump(float t, float k, Direction d)
        {
            var a = t * Math.PI;
            return (float)((0.5f * (1 - Math.Cos(a))) - (k * Math.Sin(2 * a)));
        }

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        /// <param name="t">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float BumpContinuous(float t, float k, Direction d)
        {
            t = LinearContinuous(t, k, d);
            var a = t * Math.PI;
            return (float)(Math.Sin(0.5f * a) + (k * Math.Sin(a)));
        }
    }
}
