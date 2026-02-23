using System.Collections.Generic;
using UnityEngine;

namespace ChronoCore.Rewind
{
    [System.Serializable]
        public class RewindFrame
        {
                  public Dictionary<string, RewindSnapshot> Snapshots = new Dictionary<string, RewindSnapshot>();
                  public bool IsCheckpointFrame;
                  public Vector2 CheckpointPosition;
        }
}
