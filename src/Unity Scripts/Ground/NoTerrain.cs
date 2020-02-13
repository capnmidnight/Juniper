using UnityEngine;

namespace Juniper.Ground
{
    public class NoTerrain : AbstractGround
    {
        protected override void Awake()
        {
            Debug.LogWarning("Juniper: no terrain enabled.");
        }
    }
}
