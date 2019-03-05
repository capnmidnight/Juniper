using System;
using System.Collections.Generic;

using UnityEngine;

namespace Juniper.Unity.Animation
{
    /// <summary>
    /// Different modes of animating a single value from 0 to 1.
    /// </summary>
    public enum TweenType
    {
        /// <summary>
        /// The most basic form of `tween, values increase in proportion to time.
        /// </summary>
        Linear,

        /// <summary>
        /// Values increase in proportion to the square of time.
        /// </summary>
        Quadratic,

        /// <summary>
        /// Values increase in proportion to the cube of time. This function is specifically scalled
        /// to provide the full shape of the cube function from -1 to 1.
        /// </summary>
        Cubic,

        /// <summary>
        /// Values increase along a sine wave (actually, 1 minus cosine of time scaled to π)
        /// </summary>
        Sine,

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        Bump,

        /// <summary>
        /// An inverted curve that plays both in and out.
        /// </summary>
        PingPong
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
        /// <summary>
        /// A lookup for the tween functions, so we don't have to use reflection every time we want one.
        /// </summary>
        public static readonly Dictionary<TweenType, Func<float, float, float, float>> Functions = new Dictionary<TweenType, Func<float, float, float, float>>
        {
            { TweenType.Linear, Linear },
            { TweenType.Quadratic, Quadratic },
            { TweenType.Cubic, Cubic },
            { TweenType.Sine, Sine },
            { TweenType.Bump, Bump },
            { TweenType.PingPong, PingPong }
        };

        /// <summary>
        /// The most basic form of `tween, values increase in proportion to time.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Linear(float p, float k, float d)
        {
            return p;
        }

        /// <summary>
        /// Values increase in proportion to the square of time.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Quadratic(float p, float k, float d)
        {
            return p * p;
        }

        /// <summary>
        /// Values increase in proportion to the cube of time. This function is specifically scalled
        /// to provide the full shape of the cube function from -1 to 1.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Cubic(float p, float k, float d)
        {
            var c = 2 * p - 1;
            return 0.5f * (1 + c * c * c);
        }

        /// <summary>
        /// Values increase along a sine wave (actually, 1 minus cosine of time scaled to π)
        /// </summary>
        /// <returns>The sine.</returns>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Sine(float p, float k, float d)
        {
            return 0.5f * (1 - Mathf.Cos(p * Mathf.PI));
        }

        /// <summary>
        /// Similar to <see cref="Sine"/>, but values bump in the opposite direction slightly, before
        /// the beginning and the end of the curve.
        /// </summary>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float Bump(float p, float k, float d)
        {
            return Sine(p, 0, d) - k * Mathf.Sin(p * Mathf.PI * 2);
        }

        /// <summary>
        /// An inverted curve that plays both in and out.
        /// </summary>
        /// <returns>The pong quadratic.</returns>
        /// <param name="p">The proportional value, linearly progressing from 0 to 1.</param>
        /// <param name="k">The constant value, input by the developer.</param>
        /// <param name="d">The direction value, 1 for forward, -1 for backward.</param>
        public static float PingPong(float p, float k, float d)
        {
            return 2 * p - 4 * p * p;
        }
    }
}
