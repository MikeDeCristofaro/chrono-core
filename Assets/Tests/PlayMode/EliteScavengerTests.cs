using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ChronoCore.Rewind;

[TestFixture]
public class EliteScavengerTests
{
    private GameObject bossObj;
    private EliteScavenger enemy;

    [SetUp]
    public void Setup()
    {
        bossObj = new GameObject("EliteScavenger");
        bossObj.transform.position = Vector3.zero;
        bossObj.transform.rotation = Quaternion.identity;
        enemy = bossObj.AddComponent<EliteScavenger>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(bossObj);
    }

    [Test]
    public void Shield_BlocksFrontalDamage()
    {
        int initialHealth = GetHealth();
        // Attack from the front (right side of enemy looking right)
        enemy.TakeDamage(1, new Vector2(5, 0));
        
        Assert.AreEqual(initialHealth, GetHealth(), "Frontal attack should be blocked by shield.");
    }

    [Test]
    public void Shield_AllowsSideDamage()
    {
        int initialHealth = GetHealth();
        // Attack from the back (left side)
        enemy.TakeDamage(1, new Vector2(-5, 0));
        
        Assert.AreEqual(initialHealth - 1, GetHealth(), "Back attack should bypass shield.");
    }

    [UnityTest]
    public IEnumerator Rewind_RestoresHealthAndPos()
    {
        Vector3 initialPos = bossObj.transform.position;
        int initialHealth = GetHealth();
        
        // Capture initial state
        RewindSnapshot initialSnapshot = enemy.CaptureState();
        
        // Change state
        bossObj.transform.position = new Vector3(10, 0, 0);
        enemy.TakeDamage(2, new Vector2(-5, 0));
        
        Assert.AreEqual(initialHealth - 2, GetHealth());
        
        // Restore
        enemy.RestoreState(initialSnapshot);
        
        Assert.AreEqual(initialPos, bossObj.transform.position);
        Assert.AreEqual(initialHealth, GetHealth());
        yield break;
    }

    private int GetHealth()
    {
        var field = typeof(EliteScavenger).GetField("_currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)field.GetValue(enemy);
    }
}
