using UnityEngine;

namespace Juniper.Ground
{
    public class NoTerrain : AbstractGround
    {
        protected override void Awake()
        {
            Debug.Log("Juniper: no terrain enabled.");
        }
    }
}
