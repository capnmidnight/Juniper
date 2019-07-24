using System;

namespace LowPolyAnimalPack
{
  [Serializable]
  public class MovementState : AnimalState
  {
    public float maxStateTime = 40f;
    public float moveSpeed = 3f;
    public float turnSpeed = 120f;
  }
}