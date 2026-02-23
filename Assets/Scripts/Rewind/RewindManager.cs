using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoCore.Rewind
{
    public class RewindManager : MonoBehaviour
    {
        public static RewindManager Instance { get; private set; }

        private const int BUFFER_CAPACITY = 540;
        private const int MIN_REWIND_FRAMES = 6;

        private RewindFrame[] _buffer = new RewindFrame[BUFFER_CAPACITY];
        private int _head = 0;
        private int _count = 0;
        private List<IRewindable> _rewindables = new List<IRewindable>();

        public bool IsRewinding { get; private set; }
        public float BufferedSeconds => _count / 60.0f;

        public event Action OnRewindStart;
        public event Action OnRewindStop;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                for (int i = 0; i < BUFFER_CAPACITY; i++)
                {
                    _buffer[i] = new RewindFrame();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterRewindable(IRewindable rewindable)
        {
            if (!_rewindables.Contains(rewindable))
                _rewindables.Add(rewindable);
        }

        public void DeregisterRewindable(IRewindable rewindable)
        {
            if (_rewindables.Contains(rewindable))
                _rewindables.Remove(rewindable);
        }

        public void SetHardStopPosition(Vector2 position)
        {
            CaptureFrame(true, position);
        }

        public void StartRewind()
        {
            if (IsRewinding || _count < MIN_REWIND_FRAMES) return;
            IsRewinding = true;
            OnRewindStart?.Invoke();
        }

        public void StopRewind()
        {
            if (!IsRewinding) return;
            IsRewinding = false;
            OnRewindStop?.Invoke();
        }

        private void FixedUpdate()
        {
            if (IsRewinding) TickRewind();
            else CaptureFrame(false, Vector2.zero);
        }

        private void CaptureFrame(bool isCheckpoint, Vector2 checkpointPos)
        {
            RewindFrame frame = _buffer[_head];

            frame.IsCheckpointFrame = isCheckpoint;
            frame.CheckpointPosition = checkpointPos;
            frame.Snapshots.Clear();

            foreach (var rewindable in _rewindables)
            {
                frame.Snapshots[rewindable.GetRewindableId()] = rewindable.CaptureState();
            }

            _head = (_head + 1) % BUFFER_CAPACITY;
            if (_count < BUFFER_CAPACITY) _count++;
        }

        private void TickRewind()
        {
            if (_count <= 0) { StopRewind(); return; }

            int prevIdx = (_head - 1 + BUFFER_CAPACITY) % BUFFER_CAPACITY;
            RewindFrame frame = _buffer[prevIdx];

            if (frame.IsCheckpointFrame)
            {
                StopRewind();
                return;
            }

            _head = prevIdx;
            _count--;

            foreach (var rewindable in _rewindables)
            {
                string id = rewindable.GetRewindableId();
                if (frame.Snapshots.TryGetValue(id, out RewindSnapshot snapshot))
                {
                    rewindable.RestoreState(snapshot);
                }
            }

            if (_count == 0) StopRewind();
        }
    }
}
