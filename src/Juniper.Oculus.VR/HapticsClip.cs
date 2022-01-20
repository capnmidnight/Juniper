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

namespace Oculus.VR
{
    /// <summary>
    /// A PCM buffer of data for a haptics effect.
    /// </summary>
    public class HapticsClip
    {
        /// <summary>
        /// The current number of samples in the clip.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The maximum number of samples the clip can store.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// The raw haptics data.
        /// </summary>
        public byte[] Samples { get; private set; }
        public object Mathf { get; private set; }

        public HapticsClip()
        {
            Capacity = Haptics.HapticsConfig.MaximumBufferSamplesCount;
            Samples = new byte[Capacity * Haptics.HapticsConfig.SampleSizeInBytes];
        }

        /// <summary>
        /// Creates a clip with the specified capacity.
        /// </summary>
        public HapticsClip(int capacity)
        {
            Capacity = (capacity >= 0) ? capacity : 0;
            Samples = new byte[Capacity * Haptics.HapticsConfig.SampleSizeInBytes];
        }

        /// <summary>
        /// Creates a clip with the specified data.
        /// </summary>
        public HapticsClip(byte[] samples, int samplesCount)
        {
            Samples = samples;
            Capacity = Samples.Length / Haptics.HapticsConfig.SampleSizeInBytes;
            Count = (samplesCount >= 0) ? samplesCount : 0;
        }

        /// <summary>
        /// Creates a clip by mixing the specified clips.
        /// </summary>
        public HapticsClip(HapticsClip a, HapticsClip b)
        {
            var maxCount = a.Count;
            if (b.Count > maxCount)
            {
                maxCount = b.Count;
            }

            Capacity = maxCount;
            Samples = new byte[Capacity * Haptics.HapticsConfig.SampleSizeInBytes];

            for (var i = 0; i < a.Count || i < b.Count; i++)
            {
                if (Haptics.HapticsConfig.SampleSizeInBytes == 1)
                {
                    byte sample = 0; // TODO support multi-byte samples
                    if ((i < a.Count) && (i < b.Count))
                    {
                        var v = a.Samples[i] + b.Samples[i];
                        sample = (byte)(v < 0 ? 0 : v > byte.MaxValue ? byte.MaxValue : v); // TODO support multi-byte samples
                    }
                    else if (i < a.Count)
                    {
                        sample = a.Samples[i]; // TODO support multi-byte samples
                    }
                    else if (i < b.Count)
                    {
                        sample = b.Samples[i]; // TODO support multi-byte samples
                    }

                    WriteSample(sample); // TODO support multi-byte samples
                }
            }
        }

        /// <summary>
        /// Creates a haptics clip from the specified audio clip.
        /// </summary>
        public HapticsClip(float[] samples, double frequency, int channels, int channel = 0)
        {
            InitializeFromAudioFloatTrack(samples, frequency, channels, channel);
        }

        /// <summary>
        /// Adds the specified sample to the end of the clip.
        /// </summary>
        public void WriteSample(byte sample) // TODO support multi-byte samples
        {
            if (Count >= Capacity)
            {
                //Console.Error.WriteLine("Attempted to write OVRHapticsClip sample out of range - Count:" + Count + " Capacity:" + Capacity);
                return;
            }

            if (Haptics.HapticsConfig.SampleSizeInBytes == 1)
            {
                Samples[Count * Haptics.HapticsConfig.SampleSizeInBytes] = sample; // TODO support multi-byte samples
            }

            Count++;
        }

        /// <summary>
        /// Clears the clip and resets its size to 0.
        /// </summary>
        public void Reset()
        {
            Count = 0;
        }

        private void InitializeFromAudioFloatTrack(float[] sourceData, double sourceFrequency, int sourceChannelCount, int sourceChannel)
        {
            var stepSizePrecise = (sourceFrequency + 1e-6) / Haptics.HapticsConfig.SampleRateHz;

            if (stepSizePrecise < 1.0)
            {
                return;
            }

            var stepSize = (int)stepSizePrecise;
            var stepSizeError = stepSizePrecise - stepSize;
            double accumulatedStepSizeError = 0.0f;
            var length = sourceData.Length;

            Count = 0;
            Capacity = length / sourceChannelCount / stepSize + 1;
            Samples = new byte[Capacity * Haptics.HapticsConfig.SampleSizeInBytes];

            var i = sourceChannel % sourceChannelCount;
            while (i < length)
            {
                if (Haptics.HapticsConfig.SampleSizeInBytes == 1)
                {
                    var v = Math.Abs(sourceData[i]);
                    WriteSample((byte)((v < 0 ? 0 : v > 1 ? 1 : v) * byte.MaxValue)); // TODO support multi-byte samples
                }
                i += stepSize * sourceChannelCount;
                accumulatedStepSizeError += stepSizeError;
                if ((int)accumulatedStepSizeError > 0)
                {
                    i += (int)accumulatedStepSizeError * sourceChannelCount;
                    accumulatedStepSizeError = accumulatedStepSizeError - (int)accumulatedStepSizeError;
                }
            }
        }
    }
}