using UnityEngine;

namespace Juniper.Unity.Ground
{
    public class NoTerrain : AbstractGround
    {
        protected override void InternalStart(JuniperPlatform xr)
        {
            Debug.Log("Juniper: no terrain enabled.");
        }
    }
}
