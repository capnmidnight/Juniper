/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Oculus.VR
{
    public static partial class Haptics
    {
        private class HapticsOutput : IDisposable
        {
            private class ClipPlaybackTracker
            {
                public int ReadCount { get; set; }
                public HapticsClip Clip { get; set; }

                public ClipPlaybackTracker(HapticsClip clip)
                {
                    Clip = clip;
                }
            }

            private readonly bool m_lowLatencyMode = true;
            private readonly bool m_paddingEnabled = true;
            private int m_prevSamplesQueued = 0;
            private float m_prevSamplesQueuedTime = 0;
            private int m_numPredictionHits = 0;
            private int m_numPredictionMisses = 0;
            private int m_numUnderruns = 0;
            private readonly List<ClipPlaybackTracker> m_pendingClips = new List<ClipPlaybackTracker>();
            private readonly uint m_controller = 0;
            private NativeBuffer m_nativeBuffer = new NativeBuffer(HapticsConfig.MaximumBufferSamplesCount * HapticsConfig.SampleSizeInBytes);
            private readonly HapticsClip m_paddingClip = new HapticsClip();

            public HapticsOutput(uint controller)
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    m_paddingEnabled = false;
                }
                m_controller = controller;
            }

            ~HapticsOutput()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(false);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    m_nativeBuffer?.Dispose();
                    m_nativeBuffer = null;
                }
            }

            /// <summary>
            /// The system calls this each frame to update haptics playback.
            /// </summary>
            public void Process()
            {
                var hapticsState = Plugin.GetControllerHapticsState(m_controller);

                var elapsedTime = Juniper.Units.Ticks.Seconds(DateTime.Now.Ticks) - m_prevSamplesQueuedTime;
                if (m_prevSamplesQueued > 0)
                {
                    var expectedSamples = m_prevSamplesQueued - (int)(elapsedTime * HapticsConfig.SampleRateHz + 0.5f);
                    if (expectedSamples < 0)
                    {
                        expectedSamples = 0;
                    }

                    if ((hapticsState.SamplesQueued - expectedSamples) == 0)
                    {
                        m_numPredictionHits++;
                    }
                    else
                    {
                        m_numPredictionMisses++;
                    }

                    //Console.WriteLine(hapticsState.SamplesAvailable + "a " + hapticsState.SamplesQueued + "q " + expectedSamples + "e "
                    //+ "Prediction Accuracy: " + m_numPredictionHits / (float)(m_numPredictionMisses + m_numPredictionHits));

                    if ((expectedSamples > 0) && (hapticsState.SamplesQueued == 0))
                    {
                        m_numUnderruns++;
                        //Console.Error.WriteLine("Samples Underrun (" + m_controller + " #" + m_numUnderruns + ") -"
                        //        + " Expected: " + expectedSamples
                        //        + " Actual: " + hapticsState.SamplesQueued);
                    }

                    m_prevSamplesQueued = hapticsState.SamplesQueued;
                    m_prevSamplesQueuedTime = Juniper.Units.Ticks.Seconds(DateTime.Now.Ticks);
                }

                var desiredSamplesCount = HapticsConfig.OptimalBufferSamplesCount;
                if (m_lowLatencyMode)
                {
                    var sampleRateMs = 1000.0f / HapticsConfig.SampleRateHz;
                    var elapsedMs = elapsedTime * 1000.0f;
                    var samplesNeededPerFrame = (int)Math.Ceiling(elapsedMs / sampleRateMs);
                    var lowLatencySamplesCount = HapticsConfig.MinimumSafeSamplesQueued + samplesNeededPerFrame;

                    if (lowLatencySamplesCount < desiredSamplesCount)
                    {
                        desiredSamplesCount = lowLatencySamplesCount;
                    }
                }

                if (hapticsState.SamplesQueued > desiredSamplesCount)
                {
                    return;
                }

                if (desiredSamplesCount > HapticsConfig.MaximumBufferSamplesCount)
                {
                    desiredSamplesCount = HapticsConfig.MaximumBufferSamplesCount;
                }

                if (desiredSamplesCount > hapticsState.SamplesAvailable)
                {
                    desiredSamplesCount = hapticsState.SamplesAvailable;
                }

                var acquiredSamplesCount = 0;
                var clipIndex = 0;
                while (acquiredSamplesCount < desiredSamplesCount && clipIndex < m_pendingClips.Count)
                {
                    var numSamplesToCopy = desiredSamplesCount - acquiredSamplesCount;
                    var remainingSamplesInClip = m_pendingClips[clipIndex].Clip.Count - m_pendingClips[clipIndex].ReadCount;
                    if (numSamplesToCopy > remainingSamplesInClip)
                    {
                        numSamplesToCopy = remainingSamplesInClip;
                    }

                    if (numSamplesToCopy > 0)
                    {
                        var numBytes = numSamplesToCopy * HapticsConfig.SampleSizeInBytes;
                        var dstOffset = acquiredSamplesCount * HapticsConfig.SampleSizeInBytes;
                        var srcOffset = m_pendingClips[clipIndex].ReadCount * HapticsConfig.SampleSizeInBytes;
                        Marshal.Copy(m_pendingClips[clipIndex].Clip.Samples, srcOffset, m_nativeBuffer.GetPointer(dstOffset), numBytes);

                        m_pendingClips[clipIndex].ReadCount += numSamplesToCopy;
                        acquiredSamplesCount += numSamplesToCopy;
                    }

                    clipIndex++;
                }

                for (var i = m_pendingClips.Count - 1; i >= 0 && m_pendingClips.Count > 0; i--)
                {
                    if (m_pendingClips[i].ReadCount >= m_pendingClips[i].Clip.Count)
                    {
                        m_pendingClips.RemoveAt(i);
                    }
                }

                if (m_paddingEnabled)
                {
                    var desiredPadding = desiredSamplesCount - (hapticsState.SamplesQueued + acquiredSamplesCount);
                    if (desiredPadding < (HapticsConfig.MinimumBufferSamplesCount - acquiredSamplesCount))
                    {
                        desiredPadding = (HapticsConfig.MinimumBufferSamplesCount - acquiredSamplesCount);
                    }

                    if (desiredPadding > hapticsState.SamplesAvailable)
                    {
                        desiredPadding = hapticsState.SamplesAvailable;
                    }

                    if (desiredPadding > 0)
                    {
                        var numBytes = desiredPadding * HapticsConfig.SampleSizeInBytes;
                        var dstOffset = acquiredSamplesCount * HapticsConfig.SampleSizeInBytes;
                        var srcOffset = 0;
                        Marshal.Copy(m_paddingClip.Samples, srcOffset, m_nativeBuffer.GetPointer(dstOffset), numBytes);

                        acquiredSamplesCount += desiredPadding;
                    }
                }

                if (acquiredSamplesCount > 0)
                {
                    Plugin.HapticsBuffer hapticsBuffer;
                    hapticsBuffer.Samples = m_nativeBuffer.GetPointer();
                    hapticsBuffer.SamplesCount = acquiredSamplesCount;

                    Plugin.SetControllerHaptics(m_controller, hapticsBuffer);

                    hapticsState = Plugin.GetControllerHapticsState(m_controller);
                    m_prevSamplesQueued = hapticsState.SamplesQueued;
                    m_prevSamplesQueuedTime = Juniper.Units.Ticks.Seconds(DateTime.Now.Ticks);
                }
            }

            /// <summary>
            /// Immediately plays the specified clip without waiting for any currently-playing clip to finish.
            /// </summary>
            public void Preempt(HapticsClip clip)
            {
                m_pendingClips.Clear();
                m_pendingClips.Add(new ClipPlaybackTracker(clip));
            }

            /// <summary>
            /// Enqueues the specified clip to play after any currently-playing clip finishes.
            /// </summary>
            public void Queue(HapticsClip clip)
            {
                m_pendingClips.Add(new ClipPlaybackTracker(clip));
            }

            /// <summary>
            /// Adds the samples from the specified clip to the ones in the currently-playing clip(s).
            /// </summary>
            public void Mix(HapticsClip clip)
            {
                var numClipsToMix = 0;
                var numSamplesToMix = 0;
                var numSamplesRemaining = clip.Count;

                while (numSamplesRemaining > 0 && numClipsToMix < m_pendingClips.Count)
                {
                    var numSamplesRemainingInClip = m_pendingClips[numClipsToMix].Clip.Count - m_pendingClips[numClipsToMix].ReadCount;
                    numSamplesRemaining -= numSamplesRemainingInClip;
                    numSamplesToMix += numSamplesRemainingInClip;
                    numClipsToMix++;
                }

                if (numSamplesRemaining > 0)
                {
                    numSamplesToMix += numSamplesRemaining;
                    numSamplesRemaining = 0;
                }

                if (numClipsToMix > 0)
                {
                    var mixClip = new HapticsClip(numSamplesToMix);

                    var a = clip;
                    var aReadCount = 0;

                    for (var i = 0; i < numClipsToMix; i++)
                    {
                        var b = m_pendingClips[i].Clip;
                        for (var bReadCount = m_pendingClips[i].ReadCount; bReadCount < b.Count; bReadCount++)
                        {
                            if (HapticsConfig.SampleSizeInBytes == 1)
                            {
                                byte sample = 0; // TODO support multi-byte samples
                                if ((aReadCount < a.Count) && (bReadCount < b.Count))
                                {
                                    var v = a.Samples[aReadCount] + b.Samples[bReadCount];
                                    sample = (byte)(v < 0 ? 0 : v > byte.MaxValue ? byte.MaxValue : v); // TODO support multi-byte samples
                                    aReadCount++;
                                }
                                else if (bReadCount < b.Count)
                                {
                                    sample = b.Samples[bReadCount]; // TODO support multi-byte samples
                                }

                                mixClip.WriteSample(sample); // TODO support multi-byte samples
                            }
                        }
                    }

                    while (aReadCount < a.Count)
                    {
                        if (HapticsConfig.SampleSizeInBytes == 1)
                        {
                            mixClip.WriteSample(a.Samples[aReadCount]); // TODO support multi-byte samples
                        }
                        aReadCount++;
                    }

                    m_pendingClips[0] = new ClipPlaybackTracker(mixClip);
                    for (var i = 1; i < numClipsToMix; i++)
                    {
                        m_pendingClips.RemoveAt(1);
                    }
                }
                else
                {
                    m_pendingClips.Add(new ClipPlaybackTracker(clip));
                }
            }

            public void Clear()
            {
                m_pendingClips.Clear();
            }
        }
    }
}