using System;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Events
{
    /// <summary>
    /// A component that can be used to trigger events after a set amount of time. Useful for
    /// automatically advancing between story views, or delaying action after user input.
    /// </summary>
    public class TimedEvent : MonoBehaviour
    {
        /// <summary>
        /// Whether or not the event timer should start automatically when the component comes awake.
        /// </summary>
        public bool playOnAwake;

        /// <summary>
        /// The amount of time, in seconds, to allow to elapse before triggering the event.
        /// </summary>
        public float delay;

        /// <summary>
        /// Whether or not the event timer should repeat after it has actuated.
        /// </summary>
        public bool repeat;

        /// <summary>
        /// The event to occur when the timer has finished counting down.
        /// </summary>
        public UnityEvent onTimeout = new UnityEvent();

        /// <summary>
        /// The event to occur when the timer has finished counting down.
        /// </summary>
        public event EventHandler Timeout;

        /// <summary>
        /// If <see cref="playOnAwake"/> is set to true, starts the firing timer.
        /// </summary>
        public void Awake()
        {
            if (playOnAwake)
            {
                Play();
            }
        }

        /// <summary>
        /// Stop the timer, if it's looping in a repeat.
        /// </summary>
        public void Stop()
        {
            CancelInvoke();
        }

        /// <summary>
        /// Starts the delay timer.
        /// </summary>
        public void Play()
        {
            if (repeat)
            {
                InvokeRepeating("Fire", delay, delay);
            }
            else
            {
                Invoke(nameof(Fire), delay);
            }
        }

        /// <summary>
        /// Runs a delay timer and then executes the event.
        /// </summary>
        /// <returns>The coroutine.</returns>
        private void Fire()
        {
            onTimeout?.Invoke();
            Timeout?.Invoke(this, EventArgs.Empty);
        }
    }
}
