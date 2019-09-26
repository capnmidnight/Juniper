using System.Collections.Generic;
using Juniper.Haptics;
using Juniper.Input.Pointers.Screen;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Input.Pointers.Gaze
{
    public class GazePointer :
#if TOBII
        TobiiGazePointer
#elif UNITY_XR_MAGICLEAP
        MagicLeapGazePointer
#elif UNITY_XR_ARKIT || UNITY_XR_ARCORE
        TouchGazePointer
#else
        NosePointer
#endif
    {
    }

    public abstract class AbstractGazePointer :
        AbstractScreenDevice<Unary, UnaryPointerConfiguration>
    {
        protected bool gazed, wasGazed;

        private GameObject lastTarget;

        [Range(0, 5)]
        public float gazeThreshold = 2;

        private float gazeTime;

        public override bool IsConnected { get { return true; } }

        public override bool IsButtonPressed(Unary button)
        {
            return gazed;
        }

        public override bool IsButtonDown(Unary button)
        {
            return gazed && !wasGazed;
        }

        public override bool IsButtonUp(Unary button)
        {
            return !gazed && wasGazed;
        }

        /// <summary>
        /// Disables gazing for the pointer.
        /// </summary>
        public override void OnProbeFound()
        {
            base.OnProbeFound();

            if (probe != null)
            {
                probe.CanGaze = gazeThreshold > 0;
            }
        }

        private GameObject target;

        public override void Process(JuniperPointerEventData evtData, float pixelDragThresholdSquared, List<KeyCode> keyPresses)
        {
            target = evtData.pointerCurrentRaycast.gameObject;

            base.Process(evtData, pixelDragThresholdSquared, keyPresses);
        }

        protected override void InternalUpdate()
        {
            if (target != lastTarget)
            {
                gazeTime = Time.unscaledTime;
            }
            lastTarget = target;

            wasGazed = gazed;

            if (target == null)
            {
                gazed = false;
                probe?.SetGaze(0);
            }
            else if (gazeThreshold > 0)
            {
                var deltaTime = Time.unscaledTime - gazeTime;
                gazed = gazeThreshold <= deltaTime
                    && deltaTime < (gazeThreshold + 0.125f);
                probe.SetGaze(deltaTime / gazeThreshold);
            }
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
            return this.Ensure<NoHaptics>();
        }
    }
}
