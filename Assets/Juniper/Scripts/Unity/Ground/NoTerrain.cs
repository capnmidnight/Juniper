using UnityEngine;

namespace Juniper.Ground
{
    public class NoTerrain : AbstractGround
    {
        protected override void InternalStart(XRSystem xr)
        {
            Debug.Log("Juniper: no terrain enabled.");
        }
    }
}
