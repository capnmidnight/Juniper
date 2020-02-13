using System;
using UnityEngine;

namespace E7.Native
{
    /// <summary>
    /// Native Touch's native side is continuously writing into a ring buffer. This class help you read from it.
    /// The write at native side would move the "final cursor" forward. This class is remembering the "current cursor".
    /// The distance between "current cursor" and "final cursor" signifies how many touches are available to read.
    /// 
    /// When you <see cref="NativeTouchRingBuffer.TryGetAndMoveNext(out NativeTouchData)">, it move the "current cursor" by 1, then get you the touch data.
    /// You can do it until it catches up with the final cursor.
    /// 
    /// If you skip get and moving next the touches and let the native writes too much that the ring buffer wraps around, 
    /// the <see cref="NativeTouchRingBuffer.TryGetAndMoveNext(out NativeTouchData)"> will have to start from the earliest possible touches in the ring buffer. 
    /// That is, you have missed some touches as they were overwritten already.
    /// 
    /// You can use <see cref="NativeTouchRingBuffer.DiscardAllTouches"> to instantly catch up the "current cursor" to the "final cursor".
    /// </summary>
    public class NativeTouchRingBuffer
    {
        private int currentCursor;

        public NativeTouchRingBuffer()
        {
            currentCursor = NativeTouch.finalCursor;
        }

        /// <summary>
        /// Read the data at current cursor on the ring buffer then move forward by 1.
        /// 
        /// If too many touches had passed that it loops over the ring buffer and overwritten your current cursor position,
        /// this read automatically change the cursor to be the earliest touch and start from there.
        /// </summary>
        /// <param name="ntd">When returning `false`, this is a `default` value and should not be used.</param>
        /// <returns>`true` when there is a remaining touch to get. This is to be used with `while` loop.</returns>
        /// <exception cref="InvalidOperationException">Thrown when you use this while in <see cref="NativeTouch.StartOption.fullMode">.</exception>
        public bool TryGetAndMoveNext(out NativeTouchData ntd)
        {
            if(NativeTouch.IsFullMode)
            {
                throw new InvalidOperationException("You cannot TryGetAndMoveNext() while Native Touch is in full mode. Use TryGetAndMoveNextFull() instead.");
            }
            EnterCriticalSection();
            CatchUpIfRequired();
            //Debug.Log($"{currentCursor} vs {NativeTouch.finalCursor}");
            if(currentCursor < NativeTouch.finalCursor)
            {
                int getIndex = currentCursor % NativeTouch.activeRingBufferSize;
                ntd = NativeTouch.ntdRingBuffer[getIndex];
                //Debug.Log($"Dequeued {ntd} at {getIndex}");
                ExitCriticalSection();
                currentCursor++;
                return true;
            }
            else
            {
                ExitCriticalSection();
                ntd = default(NativeTouchData);
                return false;
            }
        }

        /// <summary>
        /// Read the data at current cursor on the ring buffer then move forward by 1.
        /// 
        /// If too many touches had passed that it loops over the ring buffer and overwritten your current cursor position,
        /// this read automatically change the cursor to be the earliest touch and start from there.
        /// </summary>
        /// <param name="ntdf">When returning `false`, this is a `default` value and should not be used.</param>
        /// <returns>`true` when there is a remaining touch to get. This is to be used with `while` loop.</returns>
        /// <exception cref="InvalidOperationException">Thrown when you use this while not in <see cref="NativeTouch.StartOption.fullMode">.</exception>
        public bool TryGetAndMoveNextFull(out NativeTouchDataFull ntdf)
        {
            if(NativeTouch.IsFullMode == false)
            {
                throw new InvalidOperationException("You cannot TryGetAndMoveNextFull() while Native Touch is in minimal mode. Use TryGetAndMoveNext() instead.");
            }
            EnterCriticalSection();
            CatchUpIfRequired();
            if(currentCursor < NativeTouch.finalCursor)
            {
                int getIndex = currentCursor % NativeTouch.activeRingBufferSize;
                ntdf = NativeTouch.ntdFullRingBuffer[getIndex];
                ExitCriticalSection();
                currentCursor++;
                return true;
            }
            else
            {
                ExitCriticalSection();
                ntdf = default(NativeTouchDataFull);
                return false;
            }
        }

        /// <summary>
        /// Instantly move the read pointer in this ring buffer to the latest touch. Skipping all previous unread touches.
        /// That is, if you call <see cref="TryGetAndMoveNext(out NativeTouchData)"> immediately after this it will fail and returns `false`.
        /// </summary>
        public void DiscardAllTouches()
        {
            EnterCriticalSection();
            currentCursor = NativeTouch.finalCursor;
            ExitCriticalSection();
        }

        private void CatchUpIfRequired()
        {
            int earliestAvailable = NativeTouch.finalCursor - NativeTouch.activeRingBufferSize;
            if(currentCursor < earliestAvailable)
            {
                currentCursor = earliestAvailable;
            }
        }

        /// <summary>
        /// Native code may be writing new touches or updating the cursor. We are making sure not to read while it writes on Android where it could be concurrent.
        /// This is [Dekker's algorithm](https://en.wikipedia.org/wiki/Dekker%27s_algorithm).
        /// </summary>
        private void EnterCriticalSection()
        {
            // Only Android's touch originated on the other thread, so in iOS it should be safe to not care about mutex.
            // Please ignore my clusterfuck of sanity checks.
#if UNITY_ANDROID
            NativeTouch.dekker[0] = 1; //C# wants to enter.
            while (NativeTouch.dekker[1] == 1) //While native wants to enter
            {
                //Debug.Log($"Waiting for native to finish {NativeTouch.dekker[0]} {NativeTouch.dekker[1]} {NativeTouch.dekker[2]}");
                if (NativeTouch.dekker[2] != 0)
                {
                    //Debug.Log($"Incorrect turn {NativeTouch.dekker[0]} {NativeTouch.dekker[1]} {NativeTouch.dekker[2]}");
                    NativeTouch.dekker[0] = 0; //Don't want to enter for now
                    while (NativeTouch.dekker[2] != 0) //Not C#'s turn
                    {
                        //Busy wait, native side will give the turn to C# soon.
                        //Debug.Log($"Busy waiting {NativeTouch.dekker[0]} {NativeTouch.dekker[1]} {NativeTouch.dekker[2]}");
                    }
                    //Debug.Log($"Out of busy {NativeTouch.dekker[0]} {NativeTouch.dekker[1]} {NativeTouch.dekker[2]}");
                    NativeTouch.dekker[0] = 1; //Want to enter again, and is now entering if native do not want to enter.
                }
                //Debug.Log($"Checking again {NativeTouch.dekker[0]} {NativeTouch.dekker[1]} {NativeTouch.dekker[2]}");
            }
            //Debug.Log($"Entering critical section! {NativeTouch.dekker[0]} {NativeTouch.dekker[1]} {NativeTouch.dekker[2]}");
#endif
        }

        /// <summary>
        /// This is also a part of [Dekker's algorithm](https://en.wikipedia.org/wiki/Dekker%27s_algorithm) continued from <see cref="EnterCriticalSection">.
        /// </summary>
        private void ExitCriticalSection()
        {
#if UNITY_ANDROID
            NativeTouch.dekker[2] = 1; //Give the turn to native side.
            NativeTouch.dekker[0] = 0; //Don't want to enter any more because I am finished.
            //Debug.Log($"ENDING critical section! {NativeTouch.dekker[0]} {NativeTouch.dekker[1]} {NativeTouch.dekker[2]}");
#endif
        }
    }
}