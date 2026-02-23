using UnityEngine;

namespace ChronoCore.Rewind
{
    /// <summary>
        /// Interface for any object that can be recorded and played back by the RewindManager.
            /// </summary>
                public interface IRewindable
                {
                          /// <summary>
                          /// Captures the current state of the object into a RewindSnapshot.
                          /// </summary>
                          RewindSnapshot CaptureState();

                          /// <summary>
                          /// Restores the object's state from a previously captured RewindSnapshot.
                          /// </summary>
                          void RestoreState(RewindSnapshot snapshot);

                          /// <summary>
                          /// Returns a unique identifier for this rewindable object.
                          /// </summary>
                          string GetRewindableId();
                }
}
