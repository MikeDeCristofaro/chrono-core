using UnityEngine;
using UnityEditor;
using ChronoCore.Rewind;
using ChronoCore.Visuals;
using ChronoCore.Environment;

public class World1_GreyBox_Generator : EditorWindow
{
    private static Material neonGreenMat;

    [MenuItem("Chrono-Core/Generate World 1 Visual Prototype v2")]
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
        EnsureComponent<UpgradeManager>(managers);
        EnsureComponent<RoomTransitionController>(managers);
        EnsureComponent<RewindVisualEffect>(managers);

        // 2. World Root
        GameObject worldRoot = GameObject.Find("World_Root");
        if (worldRoot == null) worldRoot = new GameObject("World_Root");

        // 3. Parallax Background Setup
        SetupParallax(worldRoot.transform);

        // 4. Room Generation (10 Rooms) - Using Sprites instead of Cubes
        
        // Room 01: Crash Site
        GameObject room01 = CreateRoom("Room_01_CrashSite", worldRoot.transform, Vector3.zero);
        CreateSpritePlatform(room01, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        CreateFlora(room01, new Vector3(-2, -1.2f, 0));
        CreateFlora(room01, new Vector3(3, -1.2f, 0));

        // Room 02: Facility Entrance
        GameObject room02 = CreateRoom("Room_02_Facility", worldRoot.transform, new Vector3(30, 0, 0));
        CreateSpritePlatform(room02, new Vector3(0, -2, 0), new Vector3(20, 1, 1), "Floor");
        CreateAbilityUnlock(room02, new Vector3(5, 0, 0), "WallJump", Color.cyan);
        CreateFlora(room02, new Vector3(-5, -1.2f, 0));

        // Room 03: The Climb (Vertical Shaft)
        GameObject room03 = CreateRoom("Room_03_TheClimb", worldRoot.transform, new Vector3(60, 15, 0));
        CreateSpritePlatform(room03, new Vector3(-5, 0, 0), new Vector3(1, 40, 1), "LeftWall");
        CreateSpritePlatform(room03, new Vector3(5, 0, 0), new Vector3(1, 40, 1), "RightWall");
        CreateSpritePlatform(room03, new Vector3(0, -20, 0), new Vector3(11, 1, 1), "Base");
        CreateFlora(room03, new Vector3(-4.2f, -10, 0));

        // Room 08: Boss Arena
        GameObject room08 = CreateRoom("Room_08_BossArena", worldRoot.transform, new Vector3(250, 40, 0));
        CreateSpritePlatform(room08, new Vector3(0, -5, 0), new Vector3(60, 2, 1), "BossFloor");
        CreateSpritePlatform(room08, new Vector3(-30, 10, 0), new Vector3(2, 30, 1), "LeftWall");
        CreateSpritePlatform(room08, new Vector3(30, 10, 0), new Vector3(2, 30, 1), "RightWall");
        CreateThresherBoss(room08, new Vector3(0, 5, 0));

        // 5. Player Setup
        GameObject player = SetupPlayer(new Vector3(-5, 0, 0));
        
        // 6. UI Canvas Setup
        SetupUI();

        Selection.activeGameObject = player;
        Debug.Log("World 1 Visual Prototype v2 (Parallax, Flora, Sprites) generated.");
    }

    private static void SetupParallax(Transform root)
    {
        GameObject bgRoot = new GameObject("Parallax_Background");
        bgRoot.transform.SetParent(root);

        // Far Layer
        GameObject far = CreateBackgroundLayer("Far_Towers", bgRoot.transform, 0.95f, new Color(0.1f, 0.05f, 0.2f, 1f));
        // Mid Layer
        GameObject mid = CreateBackgroundLayer("Mid_Trees", bgRoot.transform, 0.8f, new Color(0.15f, 0.1f, 0.3f, 1f));
    }

    private static GameObject CreateBackgroundLayer(string name, Transform parent, float factor, Color color)
    {
        GameObject layer = new GameObject(name);
        layer.transform.SetParent(parent);
        layer.transform.localPosition = new Vector3(0, 0, 10); // Background Z
        
        SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
        sr.color = color;
        // Placeholder sprite scale
        layer.transform.localScale = new Vector3(100, 100, 1);
        
        ParallaxLayer pl = layer.AddComponent<ParallaxLayer>();
        // Reflection for factor since it's serialized private
        var so = new SerializedObject(pl);
        so.FindProperty("parallaxFactor").floatValue = factor;
        so.ApplyModifiedProperties();

        return layer;
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

            // Setup Checks
            CreateCheck(player.transform, "GroundCheck", new Vector3(0, -1, 0), "groundCheck", pc);
            CreateCheck(player.transform, "WallCheck", new Vector3(0.6f, 0, 0), "wallCheck", pc);
            CreateCheck(player.transform, "LedgeCheck", new Vector3(0.6f, 1f, 0), "ledgeCheck", pc);

            SerializedObject so = new SerializedObject(pc);
            so.FindProperty("groundLayer").intValue = LayerMask.GetMask("Default");
            so.FindProperty("wallLayer").intValue = LayerMask.GetMask("Default");
            so.ApplyModifiedProperties();
        }
        player.transform.position = spawnPos;
        return player;
    }

    private static void CreateCheck(Transform parent, string name, Vector3 pos, string propName, MonoBehaviour target)
    {
        GameObject check = new GameObject(name);
        check.transform.SetParent(parent);
        check.transform.localPosition = pos;
        SerializedObject so = new SerializedObject(target);
        so.FindProperty(propName).objectReferenceValue = check.transform;
        so.ApplyModifiedProperties();
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

            GameObject roomTitle = new GameObject("RoomTitle_UI");
            roomTitle.transform.SetParent(canvas.transform);
            roomTitle.AddComponent<ChronoCore.UI.RoomTitleUI>();
        }
    }

    private static GameObject CreateRoom(string name, Transform parent, Vector3 pos)
    {
        GameObject room = new GameObject(name);
        room.transform.SetParent(parent);
        room.transform.position = pos;
        return room;
    }

    private static void CreateSpritePlatform(GameObject parent, Vector3 pos, Vector3 scale, string name)
    {
        GameObject plat = new GameObject(name);
        plat.transform.SetParent(parent.transform);
        plat.transform.localPosition = pos;
        plat.transform.localScale = scale;
        
        SpriteRenderer sr = plat.AddComponent<SpriteRenderer>();
        if (neonGreenMat != null) sr.material = neonGreenMat;
        sr.color = new Color(0.1f, 0.4f, 0.2f, 1f); // Base biome green
        
        plat.AddComponent<BoxCollider2D>();
    }

    private static void CreateFlora(GameObject parent, Vector3 pos)
    {
        GameObject plant = new GameObject("Luminescent_Plant");
        plant.transform.SetParent(parent.transform);
        plant.transform.localPosition = pos;
        plant.transform.localScale = Vector3.one * 1.5f;

        SpriteRenderer sr = plant.AddComponent<SpriteRenderer>();
        if (neonGreenMat != null) sr.material = neonGreenMat;
        
        plant.AddComponent<CircleCollider2D>().isTrigger = true;
        plant.AddComponent<LuminescentPlant>();
    }

    private static void CreateAbilityUnlock(GameObject parent, Vector3 pos, string id, Color color)
    {
        GameObject unlock = new GameObject($"Unlock_{id}");
        unlock.transform.SetParent(parent.transform);
        unlock.transform.localPosition = pos;
        unlock.AddComponent<CircleCollider2D>().isTrigger = true;
        AbilityUnlockTrigger trigger = unlock.AddComponent<AbilityUnlockTrigger>();
        
        SerializedObject so = new SerializedObject(trigger);
        so.FindProperty("abilityId").stringValue = id;
        so.ApplyModifiedProperties();

        GameObject vis = new GameObject("Visual");
        vis.transform.SetParent(unlock.transform);
        vis.transform.localPosition = Vector3.zero;
        vis.AddComponent<SpriteRenderer>().color = color;
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
