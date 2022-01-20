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

namespace Oculus.VR
{
    public partial class Haptics
    {
        /// <summary>
        /// Determines the target format for haptics data on a specific device.
        /// </summary>
        public static class HapticsConfig
        {
            public static int SampleRateHz { get; private set; }
            public static int SampleSizeInBytes { get; private set; }
            public static int MinimumSafeSamplesQueued { get; private set; }
            public static int MinimumBufferSamplesCount { get; private set; }
            public static int OptimalBufferSamplesCount { get; private set; }
            public static int MaximumBufferSamplesCount { get; private set; }

            static HapticsConfig()
            {
                Load();
            }

            public static void Load()
            {
                var desc = Plugin.GetControllerHapticsDesc((uint)Plugin.Controller.RTouch);

                SampleRateHz = desc.SampleRateHz;
                SampleSizeInBytes = desc.SampleSizeInBytes;
                MinimumSafeSamplesQueued = desc.MinimumSafeSamplesQueued;
                MinimumBufferSamplesCount = desc.MinimumBufferSamplesCount;
                OptimalBufferSamplesCount = desc.OptimalBufferSamplesCount;
                MaximumBufferSamplesCount = desc.MaximumBufferSamplesCount;
            }
        }
    }
}