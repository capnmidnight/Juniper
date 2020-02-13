using UnityEngine;
using System.Collections;
using System;

using System.Runtime.InteropServices;
using AOT;

namespace E7.Native
{
    internal partial class NativeTouchInterface
    {
        [MonoPInvokeCallback(typeof(NativeTouch.FullDelegate))]
        internal static void NativeTouchFullCallback(NativeTouchDataFull ntd)
        {
            switch (ntd.CallbackType)
            {
                case TouchPhase.Began: fullCallbacksBegan.Invoke(ntd); break;
                case TouchPhase.Ended: fullCallbacksEnded.Invoke(ntd); break;
                case TouchPhase.Canceled: fullCallbacksCancelled.Invoke(ntd); break;
                case TouchPhase.Moved: fullCallbacksMoved.Invoke(ntd); break;
            }
        }

        [MonoPInvokeCallback(typeof(NativeTouch.MinimalDelegate))]
        internal static void NativeTouchMinimalCallback(NativeTouchData ntd)
        {
            switch (ntd.CallbackType)
            {
                case TouchPhase.Began: minimalCallbacksBegan.Invoke(ntd); break;
                case TouchPhase.Ended: minimalCallbacksEnded.Invoke(ntd); break;
                case TouchPhase.Canceled: minimalCallbacksCancelled.Invoke(ntd); break;
                case TouchPhase.Moved: minimalCallbacksMoved.Invoke(ntd); break;
            }
        }

        [MonoPInvokeCallback(typeof(NativeTouch.CheckRingBufferDelegate))]
        internal static void NativeTouchMinimalCallbackRingBuffer(int start, int count)
        {
            int currentIndex = start;
            do
            {
                //No mutex is required for this read, since this continues immediately after the native side itself writes. No one else could be writing at this moment.
                NativeTouchData ntd = NativeTouch.ntdRingBuffer[currentIndex];

                switch (ntd.CallbackType)
                {
                    case TouchPhase.Began: minimalCallbacksBegan.Invoke(ntd); break;
                    case TouchPhase.Ended: minimalCallbacksEnded.Invoke(ntd); break;
                    case TouchPhase.Canceled: minimalCallbacksCancelled.Invoke(ntd); break;
                    case TouchPhase.Moved: minimalCallbacksMoved.Invoke(ntd); break;
                }

                currentIndex = (currentIndex + 1) % NativeTouch.activeRingBufferSize;
                count--;
            }
            while (count > 0);
        }

        [MonoPInvokeCallback(typeof(NativeTouch.CheckRingBufferDelegate))]
        internal static void NativeTouchFullCallbackRingBuffer(int start, int count)
        {
            int currentIndex = start;
            do
            {
                //No mutex is required for this read, since this continues immediately after the native side itself writes. No one else could be writing at this moment.
                NativeTouchDataFull ntd = NativeTouch.ntdFullRingBuffer[currentIndex];

                switch (ntd.CallbackType)
                {
                    case TouchPhase.Began: fullCallbacksBegan.Invoke(ntd); break;
                    case TouchPhase.Ended: fullCallbacksEnded.Invoke(ntd); break;
                    case TouchPhase.Canceled: fullCallbacksCancelled.Invoke(ntd); break;
                    case TouchPhase.Moved: fullCallbacksMoved.Invoke(ntd); break;
                }

                currentIndex = (currentIndex + 1) % NativeTouch.activeRingBufferSize;
                count--;
            }
            while (count > 0);
        }

        internal static event NativeTouch.MinimalDelegate minimalCallbacksBegan;
        internal static event NativeTouch.MinimalDelegate minimalCallbacksEnded;
        internal static event NativeTouch.MinimalDelegate minimalCallbacksCancelled;
        internal static event NativeTouch.MinimalDelegate minimalCallbacksMoved;

        internal static event NativeTouch.FullDelegate fullCallbacksBegan;
        internal static event NativeTouch.FullDelegate fullCallbacksEnded;
        internal static event NativeTouch.FullDelegate fullCallbacksCancelled;
        internal static event NativeTouch.FullDelegate fullCallbacksMoved;

        internal static bool AreSomeCallbacksNull(bool isFullMode)
        {
            if (isFullMode)
            {
                if (
                   fullCallbacksBegan == null ||
                    fullCallbacksEnded == null ||
                    fullCallbacksCancelled == null ||
                    fullCallbacksMoved == null
                )
                {
                    return true;
                }
            }
            else
            {
                if (
                    minimalCallbacksBegan == null ||
                    minimalCallbacksEnded == null ||
                    minimalCallbacksCancelled == null ||
                    minimalCallbacksMoved == null
                )
                {
                    return true;
                }
            }
            return false;
        }

        internal static void ClearCallbacks()
        {
            minimalCallbacksBegan = null;
            minimalCallbacksEnded = null;
            minimalCallbacksCancelled = null;
            minimalCallbacksMoved = null;

            fullCallbacksBegan = null;
            fullCallbacksEnded = null;
            fullCallbacksCancelled = null;
            fullCallbacksMoved = null;
        }
    }
}