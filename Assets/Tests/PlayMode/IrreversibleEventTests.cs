using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class IrreversibleEventTests
{
    private GameObject managerGameObject;
    private IrreversibleEventManager manager;

    [SetUp]
    public void Setup()
    {
        managerGameObject = new GameObject("IrreversibleEventManager");
        manager = managerGameObject.AddComponent<IrreversibleEventManager>();
    }

    [TearDown]
    public void Teardown()
    {
        if (managerGameObject != null)
        {
            Object.DestroyImmediate(managerGameObject);
        }
    }

    [Test]
    public void RegisterEvent_AddsEvent()
    {
        string eventId = "test_event";
        manager.RegisterEvent(eventId);
        Assert.IsTrue(manager.IsEventTriggered(eventId));
    }

    [Test]
    public void IsEventTriggered_ReturnsFalse_IfNotTriggered()
    {
        Assert.IsFalse(manager.IsEventTriggered("non_existent_event"));
    }

    [Test]
    public void RegisterEvent_IgnoresDuplicate()
    {
        string eventId = "duplicate_event";
        manager.RegisterEvent(eventId);
        manager.RegisterEvent(eventId);
        Assert.IsTrue(manager.IsEventTriggered(eventId));
    }

    [Test]
    public void ResetAllEvents_ClearsEvents()
    {
        string eventId = "reset_event";
        manager.RegisterEvent(eventId);
        Assert.IsTrue(manager.IsEventTriggered(eventId));

        manager.ResetAllEvents();
        Assert.IsFalse(manager.IsEventTriggered(eventId));
    }

    [UnityTest]
    public IEnumerator Singleton_Instance_IsSet()
    {
        yield return null; // Wait for Awake
        Assert.IsNotNull(IrreversibleEventManager.Instance);
        Assert.AreEqual(manager, IrreversibleEventManager.Instance);
    }

    [UnityTest]
    public IEnumerator Singleton_Duplicate_Destroyed()
    {
        yield return null; // Wait for first manager Awake

        GameObject duplicateObj = new GameObject("DuplicateManager");
        duplicateObj.AddComponent<IrreversibleEventManager>();

        yield return null; // Wait for duplicate Awake

        // The duplicate component should destroy its GameObject
        Assert.IsTrue(duplicateObj == null);
    }
}
