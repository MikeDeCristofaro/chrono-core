using UnityEngine;
using UnityEditor;
using ChronoCore.Rewind;

public class World1_GreyBox_Generator : EditorWindow
{
    private static Material neonGreenMat;

    [MenuItem("Chrono-Core/Generate World 1 Visual Prototype")]
    public static void Generate()
    {
        // 0. Load Assets
        neonGreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/NeonGlow_Green.mat");

        // 1. Setup Managers
        GameObject managers = GameObject.Find("GlobalManagers");
        if (managers == null) managers = new GameObject("GlobalManagers");
        
        EnsureComponent<RewindManager>(managers);
        EnsureComponent<ChronoEnergyManager>(managers);
        EnsureComponent<IrreversibleEventManager>(managers);
        EnsureComponent<ChronoEnergyHUD>(managers);
        EnsureComponent<RoomTransitionController>(managers);
        EnsureComponent<RewindVisualEffect>(managers);

        // 2. World Root
        GameObject worldRoot = GameObject.Find("World_Root");
        if (worldRoot == null) worldRoot = new GameObject("World_Root");

        // 3. Room 01: Crash Site
        GameObject room01 = CreateRoom("Room_01_CrashSite", worldRoot.transform, Vector3.zero);
        CreateVibrantPlatform(room01, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        CreateVibrantPlatform(room01, new Vector3(-8, 5, 0), new Vector3(5, 1, 1), "Raised_Platform");
        
        // Add a "Point Light" placeholder
        CreateLightSource(room01, new Vector3(0, 2, 0), Color.green, 5f);

        // 4. Room 02: Luminescent Gate
        GameObject room02 = CreateRoom("Room_02_LuminescentGate", worldRoot.transform, new Vector3(30, 0, 0));
        CreateVibrantPlatform(room02, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Gate_Floor");
        
        for (int i = 0; i < 2; i++)
        {
            GameObject drone = new GameObject($"PatrolDrone_{i}");
            drone.transform.SetParent(room02.transform);
            drone.transform.localPosition = new Vector3(-5 + (i * 10), 1, 0);
            drone.AddComponent<PatrolDrone>();
        }

        // 5. Room 07: DescentGauntlet (Introducing Elite Scavenger)
        GameObject room07 = CreateRoom("Room_07_DescentGauntlet", worldRoot.transform, new Vector3(60, 0, 0));
        CreateVibrantPlatform(room07, new Vector3(0, -2, 0), new Vector3(40, 1, 1), "Sloped_Corridor");
        
        GameObject elite = new GameObject("EliteScavenger_Enemy");
        elite.transform.SetParent(room07.transform);
        elite.transform.localPosition = new Vector3(10, 1, 0);
        elite.AddComponent<EliteScavenger>();
        elite.AddComponent<Rigidbody2D>().freezeRotation = true;

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
        Debug.Log("World 1 Visual Prototype (Rooms 01, 02, 07) generated with Elite AI and Pulse Shaders.");
    }

    private static GameObject CreateRoom(string name, Transform parent, Vector3 pos)
    {
        GameObject room = new GameObject(name);
        room.transform.SetParent(parent);
        room.transform.position = pos;
        return room;
    }

    private static void CreateVibrantPlatform(GameObject parent, Vector3 pos, Vector3 scale, string name)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent.transform);
        cube.transform.localPosition = pos;
        cube.transform.localScale = scale;
        cube.layer = 6; 
        
        if (neonGreenMat != null)
        {
            cube.GetComponent<Renderer>().material = neonGreenMat;
        }

        Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
        cube.AddComponent<BoxCollider2D>();
    }

    private static void CreateLightSource(GameObject parent, Vector3 pos, Color color, float intensity)
    {
        GameObject lightObj = new GameObject("2D_Light_Placeholder");
        lightObj.transform.SetParent(parent.transform);
        lightObj.transform.localPosition = pos;
        // In a real URP 2D project, we would add 'Light2D' here.
        // lightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    private static void EnsureComponent<T>(GameObject obj) where T : Component
    {
        if (obj.GetComponent<T>() == null) obj.AddComponent<T>();
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) field.SetValue(obj, value);
    }
}
