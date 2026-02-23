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

        // 2. Room 01: Crash Site
        GameObject room01 = new GameObject("Room_01_CrashSite");
        CreatePlatform(room01, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        CreatePlatform(room01, new Vector3(-8, 5, 0), new Vector3(5, 1, 1), "Platform_A");
        CreatePlatform(room01, new Vector3(8, 2, 0), new Vector3(5, 1, 1), "Platform_B");
        
        // 3. Gated Door Example
        GameObject doorObj = new GameObject("GatedDoor_Example");
        doorObj.transform.SetParent(room01.transform);
        doorObj.transform.position = new Vector3(15, 0, 0);
        
        GameObject doorVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        doorVisual.transform.SetParent(doorObj.transform);
        doorVisual.transform.localScale = new Vector3(0.5f, 4f, 1f);
        doorVisual.GetComponent<Renderer>().material.color = Color.red;
        
        GatedDoor gd = doorObj.AddComponent<GatedDoor>();
        // Use reflection to set the door's private fields for the generator
        SetPrivateField(gd, "doorVisual", doorVisual);
        SetPrivateField(gd, "doorCollider", doorVisual.GetComponent<BoxCollider>());

        // 4. Room Transition Trigger (Example to Room 04)
        GameObject transitionTrigger = new GameObject("Transition_To_Room04");
        transitionTrigger.transform.SetParent(room01.transform);
        transitionTrigger.transform.position = new Vector3(18, 0, 0);
        BoxCollider2D bc = transitionTrigger.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(2, 5);
        // This will be caught by RoomTransitionController's OnTriggerEnter2D

        // 5. Spawn Player
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
        
        // 6. Spawn Enemy
        GameObject enemyObj = new GameObject("Enemy_PatrolDrone");
        enemyObj.transform.position = new Vector3(8, 3, 0);
        enemyObj.AddComponent<PatrolDrone>();
        
        Selection.activeGameObject = player;
        Debug.Log("World 1 Whitebox generated with Transitions, Gated Doors, Player, and AI.");
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
