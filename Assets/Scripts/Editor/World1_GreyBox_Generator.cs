using UnityEngine;
using UnityEditor;
using ChronoCore.Rewind;

public class World1_GreyBox_Generator : EditorWindow
{
    private static Material neonGreenMat;

    [MenuItem("Chrono-Core/Generate World 1 Final Vertical Slice")]
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
        EnsureComponent<JuiceManager>(managers);
        EnsureComponent<AudioManager>(managers);
        EnsureComponent<UpgradeManager>(managers); // NEW: Phase 4
        EnsureComponent<RoomTransitionController>(managers);
        EnsureComponent<RewindVisualEffect>(managers);

        // 2. World Root
        GameObject worldRoot = GameObject.Find("World_Root");
        if (worldRoot == null) worldRoot = new GameObject("World_Root");

        // 3. Room Generation (10 Rooms)
        
        // Room 01: Crash Site
        GameObject room01 = CreateRoom("Room_01_CrashSite", worldRoot.transform, Vector3.zero);
        CreateVibrantPlatform(room01, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        
        // Room 02: Research Facility (Wall Jump Unlock)
        GameObject room02 = CreateRoom("Room_02_Facility", worldRoot.transform, new Vector3(30, 0, 0));
        CreateVibrantPlatform(room02, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        CreateAbilityUnlock(room02, new Vector3(5, 0, 0), "WallJump", Color.cyan);
        
        // Room 03: The Climb (Vertical Shaft)
        GameObject room03 = CreateRoom("Room_03_TheClimb", worldRoot.transform, new Vector3(60, 15, 0));
        CreateVibrantPlatform(room03, new Vector3(-5, 0, 0), new Vector3(1, 40, 1), "LeftWall");
        CreateVibrantPlatform(room03, new Vector3(5, 0, 0), new Vector3(1, 40, 1), "RightWall");
        CreateVibrantPlatform(room03, new Vector3(0, -20, 0), new Vector3(11, 1, 1), "Base");

        // Room 04: Overgrown Junction
        GameObject room04 = CreateRoom("Room_04_Junction", worldRoot.transform, new Vector3(60, 40, 0));
        CreateVibrantPlatform(room04, new Vector3(0, -2, 0), new Vector3(40, 1, 1), "Floor");

        // Room 05: Hidden Lab (Stat Upgrade)
        GameObject room05 = CreateRoom("Room_05_Lab", worldRoot.transform, new Vector3(100, 40, 0));
        CreateVibrantPlatform(room05, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");

        // Room 06: Dark Corridor
        GameObject room06 = CreateRoom("Room_06_Corridor", worldRoot.transform, new Vector3(140, 40, 0));
        CreateVibrantPlatform(room06, new Vector3(0, -2, 0), new Vector3(50, 1, 1), "Floor");

        // Room 07: Elite Gauntlet
        GameObject room07 = CreateRoom("Room_07_Gauntlet", worldRoot.transform, new Vector3(190, 40, 0));
        CreateVibrantPlatform(room07, new Vector3(0, -2, 0), new Vector3(40, 1, 1), "Floor");
        CreateEliteScavenger(room07, new Vector3(10, 1, 0));

        // Room 08: Boss Arena
        GameObject room08 = CreateRoom("Room_08_BossArena", worldRoot.transform, new Vector3(250, 40, 0));
        CreateVibrantPlatform(room08, new Vector3(0, -5, 0), new Vector3(60, 2, 1), "BossFloor");
        CreateVibrantPlatform(room08, new Vector3(-30, 10, 0), new Vector3(2, 30, 1), "LeftWall");
        CreateVibrantPlatform(room08, new Vector3(30, 10, 0), new Vector3(2, 30, 1), "RightWall");
        CreateThresherBoss(room08, new Vector3(0, 5, 0));

        // Room 09: Extraction
        GameObject room09 = CreateRoom("Room_09_Extraction", worldRoot.transform, new Vector3(310, 40, 0));
        CreateVibrantPlatform(room09, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");

        // Room 10: Shortcut to Start
        GameObject room10 = CreateRoom("Room_10_Shortcut", worldRoot.transform, new Vector3(0, 20, 0));
        CreateVibrantPlatform(room10, new Vector3(0, -2, 0), new Vector3(10, 1, 1), "Floor");

        // 4. Player Setup
        GameObject player = SetupPlayer(new Vector3(-5, 0, 0));
        
        // 5. UI Canvas Setup
        SetupUI();

        Selection.activeGameObject = player;
        Debug.Log("World 1 Metroidvania Loop (10 Rooms) generated with Wall Jump mechanics and full boss sequence.");
    }

    private static GameObject SetupPlayer(Vector3 spawnPos)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            player = new GameObject("Player_Instance");
            player.tag = "Player";
            
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            player.AddComponent<CapsuleCollider2D>();
            PlayerController pc = player.AddComponent<PlayerController>();

            // Setup Checks (Ground/Wall)
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(player.transform);
            gc.transform.localPosition = new Vector3(0, -1, 0);
            
            GameObject wc = new GameObject("WallCheck");
            wc.transform.SetParent(player.transform);
            wc.transform.localPosition = new Vector3(0.6f, 0, 0);

            // Assign to serialized fields via Reflection/SerializedObject
            SerializedObject so = new SerializedObject(pc);
            so.FindProperty("groundCheck").objectReferenceValue = gc.transform;
            so.FindProperty("wallCheck").objectReferenceValue = wc.transform;
            so.FindProperty("groundLayer").intValue = LayerMask.GetMask("Default"); // Should be dedicated layer
            so.FindProperty("wallLayer").intValue = LayerMask.GetMask("Default");
            so.ApplyModifiedProperties();
        }
        player.transform.position = spawnPos;
        return player;
    }

    private static void SetupUI()
    {
        GameObject canvas = GameObject.Find("UI_Canvas");
        if (canvas == null)
        {
            canvas = new GameObject("UI_Canvas");
            Canvas c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            GameObject hud = new GameObject("ChronoEnergy_HUD");
            hud.transform.SetParent(canvas.transform);
            hud.AddComponent<ChronoEnergyUI>();
        }
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
        
        if (neonGreenMat != null) cube.GetComponent<Renderer>().material = neonGreenMat;

        Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
        cube.AddComponent<BoxCollider2D>();
    }

    private static void CreateAbilityUnlock(GameObject parent, Vector3 pos, string id, Color color)
    {
        GameObject unlock = new GameObject($"Unlock_{id}");
        unlock.transform.SetParent(parent.transform);
        unlock.transform.localPosition = pos;
        unlock.AddComponent<CircleCollider2D>().isTrigger = true;
        AbilityUnlockTrigger trigger = unlock.AddComponent<AbilityUnlockTrigger>();
        
        // Internal setup
        SerializedObject so = new SerializedObject(trigger);
        so.FindProperty("abilityId").stringValue = id;
        so.ApplyModifiedProperties();

        GameObject vis = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        vis.transform.SetParent(unlock.transform);
        vis.transform.localPosition = Vector3.zero;
        vis.transform.localScale = Vector3.one * 0.5f;
        vis.GetComponent<Renderer>().material.color = color;
    }

    private static void CreateThresherBoss(GameObject parent, Vector3 pos)
    {
        GameObject boss = new GameObject("ThresherUnit_Boss");
        boss.transform.SetParent(parent.transform);
        boss.transform.localPosition = pos;
        boss.AddComponent<ThresherBoss>();
        boss.AddComponent<SpriteRenderer>().color = Color.magenta;
        boss.AddComponent<BoxCollider2D>();
    }

    private static void CreateEliteScavenger(GameObject parent, Vector3 pos)
    {
        GameObject elite = new GameObject("EliteScavenger_Enemy");
        elite.transform.SetParent(parent.transform);
        elite.transform.localPosition = pos;
        elite.AddComponent<EliteScavenger>();
        elite.AddComponent<Rigidbody2D>().freezeRotation = true;
        elite.AddComponent<SpriteRenderer>().color = Color.red;
    }

    private static void EnsureComponent<T>(GameObject obj) where T : Component
    {
        if (obj.GetComponent<T>() == null) obj.AddComponent<T>();
    }
}
