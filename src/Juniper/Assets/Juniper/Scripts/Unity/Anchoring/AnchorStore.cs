using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace Juniper.Unity.Anchoring
{
    /// <summary>
    /// Anchors provide a means to restore the position of objects between sessions. In some AR
    /// systems, they also provide a means of prioritizing the stability of the location of specific
    /// objects. This component creates the native anchor type for any given subsystem, and save it
    /// to its native anchor store.
    /// </summary>
    [DisallowMultipleComponent]
    public class AnchorStore :
#if ARCORE
        ARCoreAnchorStore
#elif ARKit
        ARKitAnchorStore
#elif MAGIC_LEAP
        MagicLeapAnchorStore
#elif HOLOLENS
        HoloLensAnchorStore
#else
        MockAnchorStore
#endif
    {
    }
}
