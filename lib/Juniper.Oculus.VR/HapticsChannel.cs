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
    public static partial class Haptics
    {
        /// <summary>
        /// A track of haptics data that can be mixed or sequenced with another track.
        /// </summary>
        public class HapticsChannel
        {
            private HapticsOutput m_output;

            /// <summary>
            /// Constructs a channel targeting the specified output.
            /// </summary>
            public HapticsChannel(uint outputIndex)
            {
                m_output = m_outputs[outputIndex];
            }

            /// <summary>
            /// Cancels any currently-playing clips and immediatly plays the specified clip instead.
            /// </summary>
            public void Preempt(HapticsClip clip)
            {
                m_output.Preempt(clip);
            }

            /// <summary>
            /// Enqueues the specified clip to play after any currently-playing clips finish.
            /// </summary>
            public void Queue(HapticsClip clip)
            {
                m_output.Queue(clip);
            }

            /// <summary>
            /// Adds the specified clip to play simultaneously to the currently-playing clip(s).
            /// </summary>
            public void Mix(HapticsClip clip)
            {
                m_output.Mix(clip);
            }

            /// <summary>
            /// Cancels any currently-playing clips.
            /// </summary>
            public void Clear()
            {
                m_output.Clear();
            }
        }
    }
}