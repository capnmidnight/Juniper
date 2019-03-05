#if UNITY_WSA && (HOLOLENS || WINDOWSMR)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.Unity.Input
{
    public abstract class WindowsMRInputModule : AbstractUnifiedInputModule
    {
        /// <summary>
        /// Manages the input controllers, either motion controllers on WindowsMR headsets, or hand
        /// tracking on HoloLens.
        /// </summary>
        public override void UpdateModule()
        {
            base.UpdateModule();
            Pointers.Motion.MotionController.UpdateReadings();
        }
    }
}
#endif