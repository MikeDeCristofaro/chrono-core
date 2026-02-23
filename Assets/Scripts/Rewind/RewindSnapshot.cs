using UnityEngine;

namespace ChronoCore.Rewind
{
    public struct RewindSnapshot
    {
              public Vector2 Position;
              public Vector2 Velocity;
              public float Rotation;
              public int Health;
              public bool IsActive;
              public int AnimatorStateHash;
              public float AnimatorNormTime;
              public float DashCooldownTimer;
              public float IFrameTimer;
              public float ChronoEnergyAmount;
              public int CustomIntA;
              public float CustomFloatA;
              public bool CustomBoolA;
    }
}
