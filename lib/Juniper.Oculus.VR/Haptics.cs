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
    /// <summary>
    /// Plays tactile effects on a tracked VR controller.
    /// </summary>
    public static partial class Haptics
    {
        public static readonly HapticsChannel[] Channels;
        public static readonly HapticsChannel LeftChannel;
        public static readonly HapticsChannel RightChannel;

        private static readonly HapticsOutput[] m_outputs;

        static Haptics()
        {
            HapticsConfig.Load();

            m_outputs = new HapticsOutput[]
            {
                new HapticsOutput((uint)Plugin.Controller.LTouch),
                new HapticsOutput((uint)Plugin.Controller.RTouch),
            };

            Channels = new HapticsChannel[]
            {
                LeftChannel = new HapticsChannel(0),
                RightChannel = new HapticsChannel(1),
            };
        }

        /// <summary>
        /// The system calls this each frame to update haptics playback.
        /// </summary>
        public static void Process()
        {
            HapticsConfig.Load();

            for (var i = 0; i < m_outputs.Length; i++)
            {
                m_outputs[i].Process();
            }
        }
    }
}