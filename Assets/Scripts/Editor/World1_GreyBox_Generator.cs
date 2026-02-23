using UnityEngine;
using UnityEditor;
using ChronoCore.Rewind;

public class World1_GreyBox_Generator : EditorWindow
{
    [MenuItem("Chrono-Core/Generate World 1 Whitebox")]
    public static void Generate()
    {
        // Setup Layers (Programmatically if possible, but at least ensure objects are tagged)
        // For whitebox, we'll assume Layer 6 is "Ground"
        
        // Setup Room 01: Crash Site
        GameObject room01 = new GameObject("Room_01_CrashSite");
        CreatePlatform(room01, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        CreatePlatform(room01, new Vector3(-8, 5, 0), new Vector3(5, 1, 1), "Platform_A");
        CreatePlatform(room01, new Vector3(8, 2, 0), new Vector3(5, 1, 1), "Platform_B");
        
        // Setup Managers
        GameObject managers = GameObject.Find("GlobalManagers");
        if (managers == null) managers = new GameObject("GlobalManagers");
        
        if (managers.GetComponent<RewindManager>() == null) managers.AddComponent<RewindManager>();
        if (managers.GetComponent<ChronoEnergyManager>() == null) managers.AddComponent<ChronoEnergyManager>();
        if (managers.GetComponent<IrreversibleEventManager>() == null) managers.AddComponent<IrreversibleEventManager>();
        if (managers.GetComponent<ChronoEnergyHUD>() == null) managers.AddComponent<ChronoEnergyHUD>();

        // Spawn Player
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            player = new GameObject("Player_Instance");
            player.tag = "Player";
            player.transform.position = new Vector3(-5, 0, 0);
            
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            player.AddComponent<CapsuleCollider2D>();
            
            // Ground Check Child
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(player.transform);
            groundCheck.transform.localPosition = new Vector3(0, -1f, 0);
            
            PlayerController pc = player.AddComponent<PlayerController>();
            // Use reflection or serialized property to set groundCheck if needed, 
            // but for this demo script we'll just let it find it or assign it manually.
            // Actually, let's make the script assign it.
            var prop = pc.GetType().GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prop != null) prop.SetValue(pc, groundCheck.transform);
        }
        
        // Spawn Enemy in Room 01
        GameObject enemyObj = new GameObject("Enemy_PatrolDrone");
        enemyObj.transform.position = new Vector3(8, 3, 0);
        enemyObj.AddComponent<PatrolDrone>();
        
        Selection.activeGameObject = player;
        Debug.Log("World 1 Whitebox generated with Player Controller and HUD.");
    }

    private static void CreatePlatform(GameObject parent, Vector3 pos, Vector3 scale, string name)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent.transform);
        cube.transform.position = pos;
        cube.transform.localScale = scale;
        cube.layer = 6; // Assume Layer 6 is Ground
        
        Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
        cube.AddComponent<BoxCollider2D>();
    }
}
