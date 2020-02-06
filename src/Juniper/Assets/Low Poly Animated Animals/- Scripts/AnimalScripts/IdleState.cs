using UnityEngine;
using System;

namespace LowPolyAnimalPack
{
  [Serializable]
  public class IdleState : AnimalState
  {
    public float minStateTime = 20f;
    public float maxStateTime = 40f;
    [Tooltip("Chance of it choosing this state, in comparion to other state weights.")]
    public int stateWeight = 20;
  }
}