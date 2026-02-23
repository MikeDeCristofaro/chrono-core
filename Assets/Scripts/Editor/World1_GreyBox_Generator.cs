using UnityEngine;
using UnityEditor;
using ChronoCore.Rewind;

public class World1_GreyBox_Generator : EditorWindow
{
    [MenuItem("Chrono-Core/Generate World 1 Whitebox")]
    public static void Generate()
    {
        // 1. Setup Managers
        GameObject managers = GameObject.Find("GlobalManagers");
        if (managers == null) managers = new GameObject("GlobalManagers");
        
        if (managers.GetComponent<RewindManager>() == null) managers.AddComponent<RewindManager>();
        if (managers.GetComponent<ChronoEnergyManager>() == null) managers.AddComponent<ChronoEnergyManager>();
        if (managers.GetComponent<IrreversibleEventManager>() == null) managers.AddComponent<IrreversibleEventManager>();
        if (managers.GetComponent<ChronoEnergyHUD>() == null) managers.AddComponent<ChronoEnergyHUD>();
        if (managers.GetComponent<RoomTransitionController>() == null) managers.AddComponent<RoomTransitionController>();
        if (managers.GetComponent<RewindVisualEffect>() == null) managers.AddComponent<RewindVisualEffect>();

        // 2. Room 01: Crash Site
        GameObject worldRoot = GameObject.Find("World_Root");
        if (worldRoot == null) worldRoot = new GameObject("World_Root");

        GameObject room01 = new GameObject("Room_01_CrashSite");
        room01.transform.SetParent(worldRoot.transform);
        CreatePlatform(room01, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        
        // 3. Room 02: Luminescent Gate (Combat)
        GameObject room02 = new GameObject("Room_02_LuminescentGate");
        room02.transform.SetParent(worldRoot.transform);
        room02.transform.position = new Vector3(30, 0, 0);
        CreatePlatform(room02, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Gate_Floor");
        
        // Spawn 3 Scavenger Droids (Patrol Drones for now)
        for (int i = 0; i < 3; i++)
        {
            GameObject drone = new GameObject($"ScavengerDroid_{i}");
            drone.transform.SetParent(room02.transform);
            drone.transform.localPosition = new Vector3(-5 + (i * 5), 1, 0);
            drone.AddComponent<PatrolDrone>();
        }

        // 4. Room 04: Rewind Well (Puzzle)
        GameObject room04 = new GameObject("Room_04_RewindWell");
        room04.transform.SetParent(worldRoot.transform);
        room04.transform.position = new Vector3(60, 0, 0);
        CreatePlatform(room04, new Vector3(0, -5, 0), new Vector3(10, 1, 1), "Pit_Base");
        CreatePlatform(room04, new Vector3(0, 10, 0), new Vector3(10, 1, 1), "Exit_Ledge");
        
        // Gated Door in Room 04
        GameObject doorObj = new GameObject("GatedDoor_Room04");
        doorObj.transform.SetParent(room04.transform);
        doorObj.transform.localPosition = new Vector3(4, 11, 0);
        GameObject doorVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        doorVisual.transform.SetParent(doorObj.transform);
        doorVisual.transform.localScale = new Vector3(0.5f, 4f, 1f);
        doorVisual.GetComponent<Renderer>().material.color = Color.red;
        GatedDoor gd = doorObj.AddComponent<GatedDoor>();
        SetPrivateField(gd, "doorVisual", doorVisual);
        SetPrivateField(gd, "doorCollider", doorVisual.GetComponent<BoxCollider>());

        // 5. Room 08: Boss Arena (The Thresher-Unit)
        GameObject room08 = new GameObject("Room_08_BossArena");
        room08.transform.SetParent(worldRoot.transform);
        room08.transform.position = new Vector3(100, 0, 0);
        CreatePlatform(room08, new Vector3(0, -2, 0), new Vector3(40, 1, 1), "Arena_Floor");
        
        GameObject bossPlaceholder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bossPlaceholder.name = "ThresherUnit_Placeholder";
        bossPlaceholder.transform.SetParent(room08.transform);
        bossPlaceholder.transform.localPosition = new Vector3(10, 3, 0);
        bossPlaceholder.transform.localScale = new Vector3(5, 5, 5);
        bossPlaceholder.GetComponent<Renderer>().material.color = Color.black;

        // 6. Player Setup
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
            
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(player.transform);
            groundCheck.transform.localPosition = new Vector3(0, -1f, 0);
            
            PlayerController pc = player.AddComponent<PlayerController>();
            SetPrivateField(pc, "groundCheck", groundCheck.transform);
        }
        
        Selection.activeGameObject = player;
        Debug.Log("Full World 1 Grey-Box (Rooms 01, 02, 04, 08) generated successfully.");
    }

    private static void CreatePlatform(GameObject parent, Vector3 pos, Vector3 scale, string name)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent.transform);
        cube.transform.position = pos;
        cube.transform.localScale = scale;
        cube.layer = 6; 
        
        Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
        cube.AddComponent<BoxCollider2D>();
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) field.SetValue(obj, value);
    }
}
