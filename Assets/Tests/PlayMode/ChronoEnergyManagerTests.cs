using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;

public class ChronoEnergyManagerTests
{
    private GameObject managerGameObject;
    private ChronoEnergyManager energyManager;

    [SetUp]
    public void Setup()
    {
        managerGameObject = new GameObject("ChronoEnergyManager");
        energyManager = managerGameObject.AddComponent<ChronoEnergyManager>();

        // Speed up tests by reducing defaults
        // Default: maxSegments=3, secondsPerSegment=3 => 9 total capacity
        // Test: maxSegments=2, secondsPerSegment=1 => 2 total capacity
        SetPrivateField(energyManager, "maxSegments", 2);
        SetPrivateField(energyManager, "secondsPerSegment", 1f);
        SetPrivateField(energyManager, "regenDelay", 0.5f); // 0.5s delay
        SetPrivateField(energyManager, "regenRate", 1f); // 1 segment per second (total 2s to fill)

        // Reset currentEnergy to match new max capacity
        SetPrivateField(energyManager, "currentEnergy", 2f);
    }

    [TearDown]
    public void Teardown()
    {
        if (managerGameObject != null)
        {
            Object.DestroyImmediate(managerGameObject);
        }

        // Ensure static instance is cleared if DestroyImmediate didn't catch it
        // (Though DestroyImmediate usually handles Unity object references correctly)
        // We can't easily clear the private set static property without reflection if needed,
        // but let's assume standard behavior first.
    }

    [Test]
    public void InitialState_StartsFull()
    {
        Assert.AreEqual(1.0f, energyManager.CurrentEnergyNormalized, 0.001f);
        Assert.AreEqual(2, energyManager.FullSegments);
        Assert.IsTrue(energyManager.HasEnergy());
    }

    [UnityTest]
    public IEnumerator Rewind_ConsumesEnergy()
    {
        energyManager.SetRewinding(true);
        float initialEnergy = energyManager.CurrentEnergyNormalized;

        // Wait for 0.5s. Consumption should be 0.5 units. Total capacity is 2.
        // So 0.5 / 2 = 0.25 normalized consumption.
        yield return new WaitForSeconds(0.5f);

        float expectedEnergy = 1.0f - (0.5f / 2.0f);
        Assert.Less(energyManager.CurrentEnergyNormalized, initialEnergy);
        Assert.AreEqual(expectedEnergy, energyManager.CurrentEnergyNormalized, 0.1f); // Allow some delta time variance
    }

    [UnityTest]
    public IEnumerator Rewind_StopsAtZero()
    {
        energyManager.SetRewinding(true);

        // Wait for 2.5s (capacity is 2s)
        yield return new WaitForSeconds(2.5f);

        Assert.AreEqual(0f, energyManager.CurrentEnergyNormalized, 0.001f);
        Assert.IsFalse(energyManager.HasEnergy());

        // Wait more to ensure it doesn't go negative
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(0f, energyManager.CurrentEnergyNormalized, 0.001f);
    }

    [UnityTest]
    public IEnumerator Regen_StartsAfterDelay()
    {
        // Deplete some energy
        energyManager.SetRewinding(true);
        yield return new WaitForSeconds(1.0f); // -1 energy (50%)
        energyManager.SetRewinding(false);

        float depletedEnergy = energyManager.CurrentEnergyNormalized;
        Assert.Less(depletedEnergy, 1.0f);

        // Wait for 0.4s (delay is 0.5s)
        yield return new WaitForSeconds(0.4f);

        // Should not have started regenerating yet
        Assert.AreEqual(depletedEnergy, energyManager.CurrentEnergyNormalized, 0.01f);

        // Wait for another 0.2s (total 0.6s > 0.5s)
        yield return new WaitForSeconds(0.2f);

        // Should have started regenerating
        Assert.Greater(energyManager.CurrentEnergyNormalized, depletedEnergy);
    }

    [UnityTest]
    public IEnumerator Regen_ClampsAtMax()
    {
        // Deplete slightly
        energyManager.SetRewinding(true);
        yield return new WaitForSeconds(0.5f);
        energyManager.SetRewinding(false);

        // Wait for delay (0.5s) + regen time (0.5s consumed / 1.0 rate = 0.5s) + buffer
        yield return new WaitForSeconds(0.5f + 1.0f);

        Assert.AreEqual(1.0f, energyManager.CurrentEnergyNormalized, 0.001f);

        // Wait more
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(1.0f, energyManager.CurrentEnergyNormalized, 0.001f);
    }

    [UnityTest]
    public IEnumerator Interrupt_Regen_With_Rewind()
    {
        // Deplete
        energyManager.SetRewinding(true);
        yield return new WaitForSeconds(1.0f); // -1.0 energy
        energyManager.SetRewinding(false);

        // Wait for delay to pass and regen to start
        yield return new WaitForSeconds(0.5f + 0.2f); // 0.7s total. Regen for 0.2s (+0.2 energy)

        float rechargingEnergy = energyManager.CurrentEnergyNormalized;
        Assert.Greater(rechargingEnergy, 0.5f); // Started at 1.0, -0.5 normalized, +0.1 normalized

        // Start rewinding again
        energyManager.SetRewinding(true);
        yield return new WaitForSeconds(0.2f);

        Assert.Less(energyManager.CurrentEnergyNormalized, rechargingEnergy);
    }

    [UnityTest]
    public IEnumerator Singleton_EnforcesSingleInstance()
    {
         // Create a second manager
        GameObject duplicateObj = new GameObject("DuplicateManager");
        var duplicateManager = duplicateObj.AddComponent<ChronoEnergyManager>();

        yield return null; // Wait for Destroy to happen

        // duplicateObj should be destroyed (null check on Unity Object)
        Assert.IsTrue(duplicateObj == null);

        // First manager should still be the instance
        Assert.AreEqual(energyManager, ChronoEnergyManager.Instance);
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogError($"Field {fieldName} not found on {target.GetType().Name}");
        }
    }
}
