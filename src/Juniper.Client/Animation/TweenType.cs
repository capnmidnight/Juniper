using System;
using System.Collections.Generic;

using Juniper.Input;

using TweenFuncT = System.Func<float, float, Juniper.Input.Direction, float>;

namespace Juniper.Animation
{
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
    /// Implementations for each of the values in <see cref="TweenType"/>. /// All functions receive
    /// a value p, representing the proportion of the way through the animation process we are
    /// calculating. All functions take an extra `k` parameter, for scaling tertiary effects; most
    /// functions do not use it. /// All functions return a value v, mapping the value p to one of
    /// the desired 'tween shapes.
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
        public static readonly Dictionary<TweenType, TweenFuncT> Functions = new Dictionary<TweenType, TweenFuncT>(6)
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
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Linear(float p, float k, Direction d)
        {
            return p;
        }

        /// <summary>
        /// The most basic form of `tween, values increase in proportion to time.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float LinearContinuous(float p, float k, Direction d)
        {
            if (d == Direction.Forward)
            {
                return p - 1;
            }
            else
            {
                return 1 - p;
            }
        }

        /// <summary>
        /// Values increase in proportion to the square of time, meaning they start growing
        /// slowly and then grow quickly.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Quadratic(float p, float k, Direction d)
        {
            return p * p;
        }

        /// <summary>
        /// Values increase in proportion to the square of time.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float QuadraticContinuous(float p, float k, Direction d)
        {
            p = LinearContinuous(p, k, d);
            return Math.Sign(p) * Quadratic(p, k, d);
        }

        /// <summary>
        /// Values increase on a sine curve, meaning they start growing quickly and
        /// then slow down.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="k"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static float Sine(float p, float k, Direction d)
        {
            var a = p * Math.PI;
            return (float)Math.Sin(a);
        }


        /// <summary>
        /// Values increase on a sine curve, meaning they start growing quickly and
        /// then slow down.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="k"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static float SineContinuous(float p, float k, Direction d)
        {
            var a = LinearContinuous(p, k, d) * Math.PI;
            return (float)Math.Sin(a);
        }

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Bump(float p, float k, Direction d)
        {
            var a = p * Math.PI;
            return (float)(0.5f * (1 - Math.Cos(a)) - k * Math.Sin(2 * a));
        }

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float BumpContinuous(float p, float k, Direction d)
        {
            p = LinearContinuous(p, k, d);
            var a = p * Math.PI;
            return (float)(Math.Sin(0.5f * a) + k * Math.Sin(a));
        }
    }
}
