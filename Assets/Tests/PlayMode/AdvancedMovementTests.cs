using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChronoCore.Rewind;

public class AdvancedMovementTests
{
    private GameObject _player;
    private PlayerController _controller;
    private Rigidbody2D _rb;
    private GameObject _wall;

    [SetUp]
    public void Setup()
    {
        _player = new GameObject("Player");
        _player.tag = "Player";
        _rb = _player.AddComponent<Rigidbody2D>();
        _controller = _player.AddComponent<PlayerController>();
        
        // Setup checks
        GameObject gc = new GameObject("GroundCheck");
        gc.transform.SetParent(_player.transform);
        gc.transform.localPosition = new Vector3(0, -1, 0);

        GameObject wc = new GameObject("WallCheck");
        wc.transform.SetParent(_player.transform);
        wc.transform.localPosition = new Vector3(0.6f, 0, 0);

        GameObject lc = new GameObject("LedgeCheck");
        lc.transform.SetParent(_player.transform);
        lc.transform.localPosition = new Vector3(0.6f, 1f, 0);

        // Assign via reflection/serialized object would be better in a generator, 
        // but for unit tests we can use public fields if we make them public or use internal.
        // For this test, let's assume we can set them.
        var so = new UnityEditor.SerializedObject(_controller);
        so.FindProperty("groundCheck").objectReferenceValue = gc.transform;
        so.FindProperty("wallCheck").objectReferenceValue = wc.transform;
        so.FindProperty("ledgeCheck").objectReferenceValue = lc.transform;
        so.FindProperty("groundLayer").intValue = LayerMask.GetMask("Default");
        so.FindProperty("wallLayer").intValue = LayerMask.GetMask("Default");
        so.ApplyModifiedProperties();

        // Create a wall
        _wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _wall.transform.position = new Vector3(1, 0, 0);
        _wall.AddComponent<BoxCollider2D>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(_player);
        Object.Destroy(_wall);
    }

    [UnityTest]
    public IEnumerator PlayerCanWallSlide_WhenAbilityUnlocked()
    {
        _controller.canWallJump = true;
        _player.transform.position = new Vector3(0.4f, 0, 0); // Touching wall
        
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Simulate falling against wall
        _rb.linearVelocity = new Vector2(0, -10f);
        
        yield return new WaitForFixedUpdate();

        // Should be capped by wallSlideSpeed (usually 2f)
        Assert.LessOrEqual(Mathf.Abs(_rb.linearVelocity.y), 2.1f);
    }

    [UnityTest]
    public IEnumerator PlayerCanLedgeGrab_WhenAbilityUnlocked()
    {
        _controller.canLedgeClimb = true;
        
        // Position player so WallCheck hits but LedgeCheck doesn't (top of wall)
        _wall.transform.localScale = new Vector3(1, 1, 1);
        _player.transform.position = new Vector3(0.4f, 0.5f, 0);
        _rb.linearVelocity = new Vector2(0, -1f);

        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.1f);

        // Should be in hanging state (Kinematic)
        Assert.AreEqual(RigidbodyType2D.Kinematic, _rb.bodyType);
    }

    [UnityTest]
    public IEnumerator UpgradesPersistAcrossRewind()
    {
        // Setup manager
        GameObject managers = new GameObject("Managers");
        var upgradeManager = managers.AddComponent<UpgradeManager>();
        var energyManager = managers.AddComponent<ChronoEnergyManager>();
        var rewindManager = managers.AddComponent<RewindManager>();
        var irreversibleManager = managers.AddComponent<IrreversibleEventManager>();

        yield return null;

        _controller.canWallJump = false;
        
        // Unlock wall jump
        upgradeManager.UnlockAbility("WallJump");
        Assert.IsTrue(_controller.canWallJump);

        // Start rewind
        rewindManager.StartRewind();
        yield return new WaitForSeconds(0.5f);
        rewindManager.StopRewind();

        // Should STILL be unlocked because it's an Irreversible Event
        Assert.IsTrue(_controller.canWallJump);

        Object.Destroy(managers);
    }
}
