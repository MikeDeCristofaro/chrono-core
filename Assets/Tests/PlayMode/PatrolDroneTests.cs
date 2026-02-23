using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChronoCore.Rewind;

public class PatrolDroneTests
{
    private GameObject droneObject;
    private PatrolDrone drone;
    private GameObject rewindManagerObject;
    private RewindManager rewindManager;

    [SetUp]
    public void Setup()
    {
        // Setup RewindManager
        rewindManagerObject = new GameObject("RewindManager");
        rewindManager = rewindManagerObject.AddComponent<RewindManager>();

        // Setup PatrolDrone
        droneObject = new GameObject("PatrolDrone");
        drone = droneObject.AddComponent<PatrolDrone>();
    }

    [TearDown]
    public void Teardown()
    {
        if (droneObject != null)
            Object.DestroyImmediate(droneObject);
        if (rewindManagerObject != null)
            Object.DestroyImmediate(rewindManagerObject);
    }

    [UnityTest]
    public IEnumerator PatrolDrone_Moves_Correctly()
    {
        // Set initial position
        drone.transform.position = Vector3.zero;

        // Wait for a few frames
        yield return null;
        yield return null;
        yield return null;

        // Verify position changed (assuming speed > 0)
        Assert.AreNotEqual(Vector3.zero, drone.transform.position);
    }

    [UnityTest]
    public IEnumerator PatrolDrone_Takes_Damage_And_Dies()
    {
        // Initial health is 3 (default)
        drone.TakeDamage(1);

        // Access private field via reflection or check result indirectly?
        // PatrolDrone doesn't expose health publicly.
        // But TakeDamage(3) should deactivate object.

        drone.TakeDamage(2); // Total 3 damage

        yield return null;

        Assert.IsFalse(droneObject.activeSelf, "Drone should be inactive after taking lethal damage");
    }

    [UnityTest]
    public IEnumerator PatrolDrone_Rewind_Restores_Position()
    {
        // Move drone
        drone.transform.position = new Vector3(10, 0, 0);

        // Capture state via RewindManager logic (simulated)
        // Since we can't easily trigger RewindManager's FixedUpdate loop without waiting,
        // we can manually call CaptureState if we cast to IRewindable.

        // However, RewindManager automatically captures in FixedUpdate if we wait.
        // But we need to make sure RewindManager is initialized.

        // Register should happen in Start(). We need to wait for Start().
        yield return null;

        // Let's manually trigger capture to be deterministic
        // But RewindManager doesn't expose CaptureFrame publicly.
        // So we rely on IRewindable interface on the drone.

        var rewindable = drone as IRewindable;
        Assert.IsNotNull(rewindable, "PatrolDrone should implement IRewindable");

        // Capture initial state
        RewindSnapshot snapshot1 = rewindable.CaptureState();

        // Move drone
        drone.transform.position = new Vector3(20, 0, 0);

        // Restore state
        rewindable.RestoreState(snapshot1);

        // Verify position restored
        Assert.AreEqual(10f, drone.transform.position.x, 0.01f);
    }

    [UnityTest]
    public IEnumerator PatrolDrone_Rewind_Restores_Health_And_Active()
    {
        yield return null; // Wait for Start()
        var rewindable = drone as IRewindable;

        // Capture state (full health, active)
        RewindSnapshot snapshot1 = rewindable.CaptureState();

        // Take damage and die
        drone.TakeDamage(3);
        yield return null;
        Assert.IsFalse(droneObject.activeSelf);

        // Restore state
        rewindable.RestoreState(snapshot1);

        // Verify restored
        Assert.IsTrue(droneObject.activeSelf);
        // We can't check health directly unless we use reflection or expose it.
        // But activeSelf is good enough proxy for "alive".
    }
}
