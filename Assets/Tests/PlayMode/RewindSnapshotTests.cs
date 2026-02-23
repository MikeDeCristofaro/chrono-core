using NUnit.Framework;
using UnityEngine;
using ChronoCore.Rewind;

[TestFixture]
public class RewindSnapshotTests
{
    [Test]
    public void Serialization_Preserves_Values()
    {
        // Arrange
        RewindSnapshot originalSnapshot = new RewindSnapshot
        {
            Position = new Vector2(1.5f, -2.5f),
            Velocity = new Vector2(3.0f, 0.0f),
            Health = 50,
            IsActive = true,
            AnimatorStateHash = 123456,
            AnimatorNormTime = 0.5f,
            DashCooldownTimer = 1.2f,
            IFrameTimer = 0.3f,
            ChronoEnergyAmount = 100.0f,
            CustomIntA = 42,
            CustomFloatA = 3.14f,
            CustomBoolA = true
        };

        // Act
        string json = JsonUtility.ToJson(originalSnapshot);
        RewindSnapshot deserializedSnapshot = JsonUtility.FromJson<RewindSnapshot>(json);

        // Assert
        Assert.AreEqual(originalSnapshot.Position, deserializedSnapshot.Position);
        Assert.AreEqual(originalSnapshot.Velocity, deserializedSnapshot.Velocity);
        Assert.AreEqual(originalSnapshot.Health, deserializedSnapshot.Health);
        Assert.AreEqual(originalSnapshot.IsActive, deserializedSnapshot.IsActive);
        Assert.AreEqual(originalSnapshot.AnimatorStateHash, deserializedSnapshot.AnimatorStateHash);
        Assert.AreEqual(originalSnapshot.AnimatorNormTime, deserializedSnapshot.AnimatorNormTime);
        Assert.AreEqual(originalSnapshot.DashCooldownTimer, deserializedSnapshot.DashCooldownTimer);
        Assert.AreEqual(originalSnapshot.IFrameTimer, deserializedSnapshot.IFrameTimer);
        Assert.AreEqual(originalSnapshot.ChronoEnergyAmount, deserializedSnapshot.ChronoEnergyAmount);
        Assert.AreEqual(originalSnapshot.CustomIntA, deserializedSnapshot.CustomIntA);
        Assert.AreEqual(originalSnapshot.CustomFloatA, deserializedSnapshot.CustomFloatA);
        Assert.AreEqual(originalSnapshot.CustomBoolA, deserializedSnapshot.CustomBoolA);
    }

    [Test]
    public void Default_Values_Serialization()
    {
        // Arrange
        RewindSnapshot originalSnapshot = new RewindSnapshot(); // Default constructor

        // Act
        string json = JsonUtility.ToJson(originalSnapshot);
        RewindSnapshot deserializedSnapshot = JsonUtility.FromJson<RewindSnapshot>(json);

        // Assert
        Assert.AreEqual(originalSnapshot.Position, deserializedSnapshot.Position);
        Assert.AreEqual(originalSnapshot.Velocity, deserializedSnapshot.Velocity);
        Assert.AreEqual(originalSnapshot.Health, deserializedSnapshot.Health);
        Assert.AreEqual(originalSnapshot.IsActive, deserializedSnapshot.IsActive);
        Assert.AreEqual(originalSnapshot.AnimatorStateHash, deserializedSnapshot.AnimatorStateHash);
        Assert.AreEqual(originalSnapshot.AnimatorNormTime, deserializedSnapshot.AnimatorNormTime);
        Assert.AreEqual(originalSnapshot.DashCooldownTimer, deserializedSnapshot.DashCooldownTimer);
        Assert.AreEqual(originalSnapshot.IFrameTimer, deserializedSnapshot.IFrameTimer);
        Assert.AreEqual(originalSnapshot.ChronoEnergyAmount, deserializedSnapshot.ChronoEnergyAmount);
        Assert.AreEqual(originalSnapshot.CustomIntA, deserializedSnapshot.CustomIntA);
        Assert.AreEqual(originalSnapshot.CustomFloatA, deserializedSnapshot.CustomFloatA);
        Assert.AreEqual(originalSnapshot.CustomBoolA, deserializedSnapshot.CustomBoolA);
    }
}
