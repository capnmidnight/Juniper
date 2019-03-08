#if WAVEVR
using UnityEngine;
using wvr;

namespace Juniper.Unity.Display
{
    public class ViveFocusDisplayManager : AbstractDisplayManager
    {
        public override bool Install(bool reset)
        {
            reset &= Application.isEditor;

            var baseInstall = base.Install(reset);

            this.WithLock(() =>
            {
#if UNITY_MODULES_AUDIO
#if RESONANCEAUDIO
                var wasStereo = goog.stereoSpeakerModeEnabled;
                this.RemoveComponent<ResonanceAudioListener>();
#endif

                this.RemoveComponent<AudioListener>();
                listener = ComponentExt.FindAny<AudioListener>();

#if RESONANCEAUDIO
                goog = listener.EnsureComponent<ResonanceAudioListener>();
                goog.stereoSpeakerModeEnabled = wasStereo;
#endif
#endif

                var rend = this.EnsureComponent<WaveVR_Render>();
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

                var tracker = this.EnsureComponent<WaveVR_DevicePoseTracker>().Value;
                tracker.type = WVR_DeviceType.WVR_DeviceType_HMD;
                tracker.trackPosition = true;
                tracker.EnableNeckModel = true;
                tracker.trackRotation = true;
                tracker.timing = WVR_TrackTiming.WhenNewPoses;
            });

            return baseInstall;
        }

        public override void Uninstall()
        {
            this.RemoveComponent<WaveVR_DevicePoseTracker>();
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
