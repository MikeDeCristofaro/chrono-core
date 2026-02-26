using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChronoCore.Rewind;

namespace ChronoCore.Rewind.Tests
{
    public class RewindOptimizationTests
    {
        private GameObject _managerObj;
        private RewindManager _manager;
        private MockRewindable _rewindable;

        [SetUp]
        public void Setup()
        {
            _managerObj = new GameObject("RewindManager");
            _manager = _managerObj.AddComponent<RewindManager>();
            _rewindable = new MockRewindable();
            _manager.RegisterRewindable(_rewindable);
        }

        [TearDown]
        public void Teardown()
        {
            if (_managerObj != null) Object.DestroyImmediate(_managerObj);
        }

        [UnityTest]
        public IEnumerator VerifyRewindFrameReuse()
        {
            // Get private fields via reflection
            FieldInfo bufferField = typeof(RewindManager).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
            // const fields are static
            FieldInfo capacityField = typeof(RewindManager).GetField("BUFFER_CAPACITY", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
            MethodInfo captureMethod = typeof(RewindManager).GetMethod("CaptureFrame", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(bufferField, "Could not find _buffer field");
            Assert.IsNotNull(capacityField, "Could not find BUFFER_CAPACITY field");
            Assert.IsNotNull(captureMethod, "Could not find CaptureFrame method");

            RewindFrame[] buffer = (RewindFrame[])bufferField.GetValue(_manager);
            int capacity = (int)capacityField.GetValue(null);

            // Capture one frame to ensure buffer[0] is populated
            captureMethod.Invoke(_manager, new object[] { false, Vector2.zero });

            // Store the reference of the first frame
            RewindFrame firstFrameReference = buffer[0];

            Assert.IsNotNull(firstFrameReference, "Buffer[0] should not be null after one capture");

            // Capture enough frames to wrap around the buffer
            // To overwrite index 0, we need to advance _head until it wraps back to 0.
            // After 1 call, _head is 1. To get back to 0, we need 'capacity' more calls?
            // Wait. Buffer size is capacity.
            // Call 1: _head = 0 -> writes to [0], _head becomes 1.
            // Call 2: _head = 1 -> writes to [1], _head becomes 2.
            // ...
            // Call capacity: _head = capacity-1 -> writes to [capacity-1], _head becomes 0.
            // Call capacity+1: _head = 0 -> writes to [0], _head becomes 1.

            // So we need 'capacity' more calls to overwrite [0] again.

            for (int i = 0; i < capacity; i++)
            {
                _rewindable.Counter++;
                captureMethod.Invoke(_manager, new object[] { false, Vector2.zero });
            }

            // Now buffer[0] should have been overwritten.
            RewindFrame newFrameReference = buffer[0];

            // Verify data is correct
            Assert.IsTrue(newFrameReference.Snapshots.ContainsKey(_rewindable.GetRewindableId()));
            Assert.AreEqual(_rewindable.Counter, newFrameReference.Snapshots[_rewindable.GetRewindableId()].CustomIntA);

            // This assertion verifies the optimization:
            // If optimized, references are same.
            // If not optimized (current state), references are different.
            Assert.AreSame(firstFrameReference, newFrameReference, "RewindFrame object was not reused! Allocation detected.");

            yield return null;
        }

        private class MockRewindable : IRewindable
        {
            public int Counter = 0;
            private string _id = "MockRewindable";

            public string GetRewindableId() => _id;

            public RewindSnapshot CaptureState()
            {
                return new RewindSnapshot { CustomIntA = Counter };
            }

            public void RestoreState(RewindSnapshot snapshot)
            {
                Counter = snapshot.CustomIntA;
            }
        }
    }
}
