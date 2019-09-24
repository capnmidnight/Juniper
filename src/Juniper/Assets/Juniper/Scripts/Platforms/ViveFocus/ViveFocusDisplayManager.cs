#if WAVEVR
using UnityEngine;

using wvr;

namespace Juniper.Display
{
    public class ViveFocusDisplayManager : AbstractDisplayManager
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            this.WithLock(() =>
            {
                var rend = this.Ensure<WaveVR_Render>();
                if (rend.IsNew)
                {
                    if (!rend.Value.isExpanded)
                    {
                        WaveVR_Render.Expand(rend);
                    }
                    rend.Value.cpuPerfLevel = WaveVR_Utils.WVR_PerfLevel.Maximum;
                    rend.Value.gpuPerfLevel = WaveVR_Utils.WVR_PerfLevel.Maximum;
                }

                rend.Value.origin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;

                var tracker = this.Ensure<WaveVR_DevicePoseTracker>().Value;
                tracker.type = WVR_DeviceType.WVR_DeviceType_HMD;
                tracker.trackPosition = true;
                tracker.EnableNeckModel = true;
                tracker.trackRotation = true;
                tracker.timing = WVR_TrackTiming.WhenNewPoses;

#if UNITY_MODULES_AUDIO
#if UNITY_GOOGLE_RESONANCE_AUDIO
                var wasStereo = goog.stereoSpeakerModeEnabled;
                this.Remove<ResonanceAudioListener>();
#endif

                this.Remove<AudioListener>();
                Find.Any(out listener);

#if UNITY_GOOGLE_RESONANCE_AUDIO
                goog = listener.Ensure<ResonanceAudioListener>();
                goog.stereoSpeakerModeEnabled = wasStereo;
#endif
#endif
            });
        }

        public override void Uninstall()
        {
            this.Remove<WaveVR_DevicePoseTracker>();
            var render = GetComponent<WaveVR_Render>();
            if (render != null && render.isExpanded)
            {
                WaveVR_Render.Collapse(render);
                render.Destroy();
            }

            base.Uninstall();
        }
    }
}
#endif
