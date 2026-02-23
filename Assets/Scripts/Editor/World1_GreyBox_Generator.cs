using UnityEngine;
using UnityEditor;
using ChronoCore.Rewind;
using ChronoCore.Visuals;
using ChronoCore.Environment;
using ChronoCore.AI;

public class World1_GreyBox_Generator : EditorWindow
{
    private static Material neonGreenMat;

    [MenuItem("Chrono-Core/Generate World 1 Combat Expansion")]
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

        // 3. Parallax
        SetupParallax(worldRoot.transform);

        // 4. Room 01: Aerial Combat Test
        GameObject room01 = CreateRoom("Room_01_AerialCombat", worldRoot.transform, Vector3.zero);
        CreateSpritePlatform(room01, new Vector3(0, -2, 0), new Vector3(40, 1, 1), "Floor");
        CreateFlyingDrone(room01, new Vector3(10, 10, 0));
        CreateFlyingDrone(room01, new Vector3(-10, 10, 0));

        // 5. Room 02: Elite with Hazards
        GameObject room02 = CreateRoom("Room_02_Hazards", worldRoot.transform, new Vector3(50, 0, 0));
        CreateSpritePlatform(room02, new Vector3(0, -2, 0), new Vector3(30, 1, 1), "Floor");
        CreateEliteScavenger(room02, new Vector3(10, 1, 0));

        // 6. Player Setup
        GameObject player = SetupPlayer(new Vector3(-5, 0, 0));
        SetupUI();

        Selection.activeGameObject = player;
        Debug.Log("World 1 Combat Expansion generated.");
    }

    private static void SetupParallax(Transform root)
    {
        if (GameObject.Find("Parallax_Background") != null) return;
        GameObject bgRoot = new GameObject("Parallax_Background");
        bgRoot.transform.SetParent(root);
        CreateBackgroundLayer("Far_Towers", bgRoot.transform, 0.95f, new Color(0.1f, 0.05f, 0.2f, 1f));
        CreateBackgroundLayer("Mid_Trees", bgRoot.transform, 0.8f, new Color(0.15f, 0.1f, 0.3f, 1f));
    }

    private static GameObject CreateBackgroundLayer(string name, Transform parent, float factor, Color color)
    {
        GameObject layer = new GameObject(name);
        layer.transform.SetParent(parent);
        layer.transform.localPosition = new Vector3(0, 0, 10);
        SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
        sr.color = color;
        layer.transform.localScale = new Vector3(100, 100, 1);
        ParallaxLayer pl = layer.AddComponent<ParallaxLayer>();
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

            CreateCheck(player.transform, "GroundCheck", new Vector3(0, -1, 0), "groundCheck", pc);
            CreateCheck(player.transform, "WallCheck", new Vector3(0.6f, 0, 0), "wallCheck", pc);
            CreateCheck(player.transform, "LedgeCheck", new Vector3(0.6f, 1f, 0), "ledgeCheck", pc);
            
            // Fire Point
            GameObject fp = new GameObject("GrenadeFirePoint");
            fp.transform.SetParent(player.transform);
            fp.transform.localPosition = new Vector3(0.5f, 0, 0);
            
            SerializedObject so = new SerializedObject(pc);
            so.FindProperty("grenadeFirePoint").objectReferenceValue = fp.transform;
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
        if (GameObject.Find("UI_Canvas") != null) return;
        GameObject canvas = new GameObject("UI_Canvas");
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
        sr.color = new Color(0.1f, 0.4f, 0.2f, 1f);
        plat.AddComponent<BoxCollider2D>();
    }

    private static void CreateFlyingDrone(GameObject parent, Vector3 pos)
    {
        GameObject drone = new GameObject("Flying_Drone");
        drone.transform.SetParent(parent.transform);
        drone.transform.localPosition = pos;
        drone.AddComponent<FlyingDrone>();
        drone.AddComponent<SpriteRenderer>().color = Color.yellow;
        drone.AddComponent<CircleCollider2D>();
    }

    private static void CreateEliteScavenger(GameObject parent, Vector3 pos)
    {
        GameObject elite = new GameObject("EliteScavenger_Enemy");
        elite.transform.SetParent(parent.transform);
        elite.transform.localPosition = pos;
        elite.AddComponent<EliteScavenger>();
        elite.AddComponent<Rigidbody2D>().freezeRotation = true;
        elite.AddComponent<SpriteRenderer>().color = Color.red;
        elite.AddComponent<BoxCollider2D>();
    }

    private static void EnsureComponent<T>(GameObject obj) where T : Component
    {
        if (obj.GetComponent<T>() == null) obj.AddComponent<T>();
    }
}
