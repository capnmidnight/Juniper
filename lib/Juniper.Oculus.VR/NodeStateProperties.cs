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

using System.Numerics;

namespace Oculus.VR
{
    public static class NodeStateProperties
    {
        public static bool IsHmdPresent()
        {
            return Plugin.hmdPresent;
        }

        public static bool GetNodeStatePropertyVector3(NodeStatePropertyType propertyType, Plugin.Node ovrpNodeType, Plugin.ProcessingStep stepType, out Vector3 retVec)
        {
            retVec = Vector3.Zero;
            switch (propertyType)
            {
                case NodeStatePropertyType.Acceleration:
                if (Manager.loadedXRDevice == Manager.XRDevice.Oculus)
                {
                    retVec = Plugin.GetNodeAcceleration(ovrpNodeType, stepType).FromFlippedZVector3f();
                    return true;
                }
                break;

                case NodeStatePropertyType.AngularAcceleration:
                if (Manager.loadedXRDevice == Manager.XRDevice.Oculus)
                {
                    retVec = Plugin.GetNodeAngularAcceleration(ovrpNodeType, stepType).FromFlippedZVector3f();
                    return true;
                }
                break;

                case NodeStatePropertyType.Velocity:
                if (Manager.loadedXRDevice == Manager.XRDevice.Oculus)
                {
                    retVec = Plugin.GetNodeVelocity(ovrpNodeType, stepType).FromFlippedZVector3f();
                    return true;
                }
                break;

                case NodeStatePropertyType.AngularVelocity:
                if (Manager.loadedXRDevice == Manager.XRDevice.Oculus)
                {
                    retVec = Plugin.GetNodeAngularVelocity(ovrpNodeType, stepType).FromFlippedZVector3f();
                    return true;
                }
                break;

                case NodeStatePropertyType.Position:
                if (Manager.loadedXRDevice == Manager.XRDevice.Oculus)
                {
                    retVec = Plugin.GetNodePose(ovrpNodeType, stepType).ToOVRPose().Position;
                    return true;
                }
                break;
            }

            return false;
        }

        public static bool GetNodeStatePropertyQuaternion(NodeStatePropertyType propertyType, Plugin.Node ovrpNodeType, Plugin.ProcessingStep stepType, out Quaternion retQuat)
        {
            retQuat = Quaternion.Identity;
            switch (propertyType)
            {
                case NodeStatePropertyType.Orientation:
                if (Manager.loadedXRDevice == Manager.XRDevice.Oculus)
                {
                    retQuat = Plugin.GetNodePose(ovrpNodeType, stepType).ToOVRPose().Orientation;
                    return true;
                }
                break;
            }
            return false;
        }

    }
}