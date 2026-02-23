using UnityEngine;
using UnityEditor;
using ChronoCore.Rewind;

public class World1_GreyBox_Generator : EditorWindow
{
    [MenuItem("Chrono-Core/Generate World 1 Whitebox")]
    public static void Generate()
    {
        // Setup Room 01: Crash Site
        GameObject room01 = new GameObject("Room_01_CrashSite");
        CreatePlatform(room01, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        CreatePlatform(room01, new Vector3(-8, 5, 0), new Vector3(5, 1, 1), "Platform_A");
        CreatePlatform(room01, new Vector3(8, 2, 0), new Vector3(5, 1, 1), "Platform_B");
        
        // Spawn Player
        GameObject player = new GameObject("Player_Placeholder");
        player.transform.position = new Vector3(-5, 0, 0);
        player.AddComponent<Rigidbody2D>();
        player.tag = "Player";
        
        // Setup Managers
        GameObject managers = new GameObject("GlobalManagers");
        managers.AddComponent<RewindManager>();
        managers.AddComponent<ChronoEnergyManager>();
        managers.AddComponent<IrreversibleEventManager>();
        
        // Spawn Enemy in Room 01
        GameObject enemyObj = new GameObject("Enemy_PatrolDrone");
        enemyObj.transform.position = new Vector3(8, 3, 0);
        enemyObj.AddComponent<PatrolDrone>();
        
        Selection.activeGameObject = room01;
        Debug.Log("World 1 Whitebox generated successfully.");
    }

    private static void CreatePlatform(GameObject parent, Vector3 pos, Vector3 scale, string name)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent.transform);
        cube.transform.position = pos;
        cube.transform.localScale = scale;
        
        // Add 2D Collider for our 2D game
        Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
        cube.AddComponent<BoxCollider2D>();
    }
}
