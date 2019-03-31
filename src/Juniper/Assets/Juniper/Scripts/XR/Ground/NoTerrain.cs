using UnityEngine;

namespace Juniper.Unity.Ground
{
    public class NoTerrain : AbstractGround
    {
        protected override void Awake()
        {
            Debug.Log("Juniper: no terrain enabled.");
        }
    }
}
