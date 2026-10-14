#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// MANSION DOORS & PUZZLES EDITOR TOOL
/// Menu: MirrorHouse → Setup Connecting Doors & Horror Clues (Permanent)
/// 
/// Công dụng:
/// 1. Tự động chia 2 vách ngăn phòng kín thành lối đi có lanh-tô và gắn cửa Victorian:
///    - Cửa phòng ngủ Emily <-> Thư viện (West Rooms)
///    - Cửa phòng ăn <-> Nhà kho (East Rooms)
/// 2. Tạo 3 tờ giấy manh mối (Clue Notes) trên bàn với cốt truyện kinh dị.
/// 3. Cập nhật 3 két sắt với mã số chuẩn (1894, 4219, 5731) và ẩn mã khỏi prompt tương tác.
/// 4. Đánh dấu scene Dirty để lưu lại vĩnh viễn (Ctrl+S).
/// </summary>
[InitializeOnLoad]
public class MansionDoorsAndPuzzlesEditor : EditorWindow
{
    static MansionDoorsAndPuzzlesEditor()
    {
        EditorApplication.delayCall += AutoEnsureDoorsInEditor;
        EditorSceneManager.sceneOpened += (scene, mode) => AutoEnsureDoorsInEditor();
    }

    private static void AutoEnsureDoorsInEditor()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main") return;

        // Kiểm tra xem cửa, manh mối, hoặc vách ngăn có cần cập nhật/hàn kín hay không
        bool needsRebake = false;
        if (GameObject.Find("Door_Bedroom_Library") == null ||
            GameObject.Find("Door_Dining_Storage") == null ||
            GameObject.Find("Clue_Note_Emily") == null)
        {
            needsRebake = true;
        }
        else
        {
            GameObject eastWall = GameObject.Find("Wall_East_Divider_Inner");
            GameObject westWall = GameObject.Find("Wall_West_Divider_Inner");
            if ((eastWall != null && Mathf.Abs(eastWall.transform.localScale.x - 6.90f) > 0.05f) ||
                (westWall != null && Mathf.Abs(westWall.transform.localScale.x - 6.10f) > 0.05f))
            {
                Debug.Log("[MansionDoorsAndPuzzlesEditor] Phát hiện vách ngăn chưa tối ưu khe nối hoặc độ rộng cửa. Đang tự động cập nhật...");
                needsRebake = true;
            }
        }

        if (needsRebake)
        {
            Debug.Log("[MansionDoorsAndPuzzlesEditor] Đang tự động bake cửa thông phòng & hàn kín vách tường Level_Main...");
            ExecuteSetupInternal(silent: true);
        }
    }

    [MenuItem("MirrorHouse/Setup Connecting Doors & Horror Clues (Permanent)", false, 20)]
    public static void SetupDoorsAndPuzzles()
    {
        ExecuteSetupInternal(silent: false);
    }

    private static void ExecuteSetupInternal(bool silent)
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main")
        {
            if (silent) return;

            bool proceed = EditorUtility.DisplayDialog(
                "Mở Scene Level_Main",
                $"Scene hiện tại là '{activeScene.name}'.\n\nBạn có muốn mở 'Assets/Scenes/Level_Main.unity' để thiết lập không?",
                "Mở Level_Main",
                "Hủy"
            );

            if (proceed)
            {
                string scenePath = "Assets/Scenes/Level_Main.unity";
                if (System.IO.File.Exists(scenePath))
                {
                    activeScene = EditorSceneManager.OpenScene(scenePath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy scene Assets/Scenes/Level_Main.unity!", "OK");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        // 1. Tải materials & prefabs
        Material matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Wall_Dark.mat");
        Material matPaper = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Clue_Paper.mat");
        GameObject doorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/Victorian_Door.prefab");

        float H = 3.5f;
        float T = 0.35f;

        // 2. Chế tạo Cửa 1: Phòng ngủ Emily <-> Thư viện (West Rooms at Z = 0)
        SetupWestDoor(H, T, matWall, doorPrefab);

        // 3. Chế tạo Cửa 2: Phòng ăn <-> Nhà kho (East Rooms at Z = 0)
        SetupEastDoor(H, T, matWall, doorPrefab);

        // 4. Thiết lập 3 Két sắt mật mã (Safes)
        SetupSafes();

        // 5. Chế tạo 3 Manh mối kinh dị (Clue Notes)
        SetupClueNotes(matPaper);

        // Đánh dấu Scene Dirty và LƯU TRỰC TIẾP VÀO SCENE FILE
        EditorSceneManager.MarkSceneDirty(activeScene);
        bool saved = EditorSceneManager.SaveScene(activeScene);

        string summary = "✅ THIẾT LẬP HOÀN TẤT VÀ ĐÃ BAKE VÀO SCENE LEVEL_MAIN!\n\n" +
            "🚪 2 CỬA THÔNG PHÒNG MỚI (Thấy rõ trong Scene view Edit mode & Play mode):\n" +
            "  1. Door_Bedroom_Library: Phòng ngủ Emily <-> Thư viện (X: -9.5, Z: 0)\n" +
            "  2. Door_Dining_Storage: Phòng ăn <-> Nhà kho (X: 9.5, Z: 0)\n\n" +
            "🔐 3 KÉT SẮT & MẬT MÃ:\n" +
            "  1. Két phòng ngủ: Mã 1894 (Thưởng chìa thư viện)\n" +
            "  2. Két thư viện: Mã 4219 (Thưởng chìa nhà kho)\n" +
            "  3. Két nhà kho: Mã 5731 (Thưởng Dao tế Mirror Dagger)\n\n" +
            "📜 3 MANH MỐI KINH DỊ:\n" +
            "  1. Clue_Note_Emily: Trong ngăn kéo bàn phòng ngủ Draw 1_low (Nhật ký năm 1894)\n" +
            "  2. Clue_Note_Library: Trên bàn thư viện (Cổ thư 4219)\n" +
            "  3. Clue_Note_Storage: Dưới chân bàn ăn (Vệt cào máu 5731)\n\n" +
            $"💾 File Scene 'Level_Main.unity' đã được tự động lưu vĩnh viễn! (Saved: {saved})";

        Debug.Log($"[MansionDoorsAndPuzzlesEditor]\n{summary}");
        if (!silent)
        {
            EditorUtility.DisplayDialog("Mirror House - Setup Hoàn Tất", summary, "Tuyệt vời!");
        }
    }

    private static void SetupWestDoor(float H, float T, Material matWall, GameObject doorPrefab)
    {
        // Xóa hoặc ẩn bức tường kín cũ
        GameObject oldSolidWall = GameObject.Find("Wall_Divider_West_Rooms");
        if (oldSolidWall != null)
        {
            Undo.DestroyObjectImmediate(oldSolidWall);
        }

        // Tạo 2 mảng tường hai bên cửa và lanh-tô phía trên (Hàn kín khít với tường hành lang Tây X = -2.7m, ô cửa 1.35m)
        CreateOrReplaceBox("Wall_West_Divider_Inner", new Vector3(-5.775f, H / 2, 0f), new Vector3(6.10f, H, T), matWall);
        CreateOrReplaceBox("Wall_West_Divider_Outer", new Vector3(-13.5875f, H / 2, 0f), new Vector3(6.825f, H, T), matWall);
        CreateOrReplaceBox("Lintel_Bedroom_Library_Door", new Vector3(-9.5f, 2.85f, 0f), new Vector3(1.35f, 1.3f, T), matWall);

        // Cửa thông phòng
        GameObject doorObj = GameObject.Find("Door_Bedroom_Library");
        if (doorObj == null)
        {
            if (doorPrefab != null)
            {
                doorObj = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorObj.name = "Door_Bedroom_Library";
                doorObj.transform.position = new Vector3(-9.5f, 0f, 0f);
                doorObj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                Undo.RegisterCreatedObjectUndo(doorObj, "Create Door_Bedroom_Library");
            }
            else
            {
                doorObj = CreateFallbackDoor("Door_Bedroom_Library", new Vector3(-9.5f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
            }
        }

        doorObj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        DoorInteractive doorComp = doorObj.GetComponent<DoorInteractive>();
        if (doorComp == null) doorComp = doorObj.AddComponent<DoorInteractive>();
        doorComp.SetLocked(false);
        SerializedObject soWest = new SerializedObject(doorComp);
        soWest.FindProperty("openAngle").floatValue = -90f;
        soWest.ApplyModifiedProperties();
    }

    private static void SetupEastDoor(float H, float T, Material matWall, GameObject doorPrefab)
    {
        // Xóa bức tường kín cũ
        GameObject oldSolidWall = GameObject.Find("Wall_Divider_East_Rooms");
        if (oldSolidWall != null)
        {
            Undo.DestroyObjectImmediate(oldSolidWall);
        }

        // Tạo 2 mảng tường hai bên cửa và lanh-tô phía trên (Hàn kín khít với tường hành lang Đông X = 1.92m, ô cửa 1.35m)
        CreateOrReplaceBox("Wall_East_Divider_Inner", new Vector3(5.375f, H / 2, 0f), new Vector3(6.90f, H, T), matWall);
        CreateOrReplaceBox("Wall_East_Divider_Outer", new Vector3(13.5875f, H / 2, 0f), new Vector3(6.825f, H, T), matWall);
        CreateOrReplaceBox("Lintel_Dining_Storage_Door", new Vector3(9.5f, 2.85f, 0f), new Vector3(1.35f, 1.3f, T), matWall);

        // Cửa thông phòng
        GameObject doorObj = GameObject.Find("Door_Dining_Storage");
        if (doorObj == null)
        {
            if (doorPrefab != null)
            {
                doorObj = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorObj.name = "Door_Dining_Storage";
                doorObj.transform.position = new Vector3(9.5f, 0f, 0f);
                doorObj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                Undo.RegisterCreatedObjectUndo(doorObj, "Create Door_Dining_Storage");
            }
            else
            {
                doorObj = CreateFallbackDoor("Door_Dining_Storage", new Vector3(9.5f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
            }
        }

        doorObj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        DoorInteractive doorComp = doorObj.GetComponent<DoorInteractive>();
        if (doorComp == null) doorComp = doorObj.AddComponent<DoorInteractive>();
        doorComp.SetLocked(false);
        SerializedObject soEast = new SerializedObject(doorComp);
        soEast.FindProperty("openAngle").floatValue = -90f;
        soEast.ApplyModifiedProperties();
    }

    private static void SetupSafes()
    {
        ConfigureSafeSO("Safe_Bedroom", "safe_bedroom", "1894", "key_library",
            "KÉT SẮT PHÒNG NGỦ", "Két Sắt Cổ (Nhấn E để nhập mã)");

        ConfigureSafeSO("Safe_Library", "safe_library", "4219", "key_storage",
            "KÉT BÍ MẬT THƯ VIỆN", "Két Sắt Cổ Thư Viện (Nhấn E để nhập mã)");

        ConfigureSafeSO("Safe_Storage_Dagger", "chest_storage", "5731", "key_ritual_dagger",
            "KÉT KIM LOẠI NHÀ KHO", "Két Kim Loại Nhà Kho (Nhấn E để nhập mã)");
    }

    private static void ConfigureSafeSO(string name, string lockId, string code, string rewardKey, string displayName, string prompt)
    {
        GameObject safeObj = GameObject.Find(name);
        if (safeObj == null) return;

        Lockable lockable = safeObj.GetComponent<Lockable>();
        if (lockable == null) lockable = safeObj.AddComponent<Lockable>();

        SerializedObject so = new SerializedObject(lockable);
        so.FindProperty("lockType").enumValueIndex = (int)LockType.Code;
        so.FindProperty("lockId").stringValue = lockId;
        so.FindProperty("correctCode").stringValue = code;
        so.FindProperty("rewardKeyId").stringValue = rewardKey;
        so.FindProperty("lockDisplayName").stringValue = displayName;
        so.FindProperty("interactionPrompt").stringValue = prompt;
        so.FindProperty("interactionRange").floatValue = 2.8f;
        so.ApplyModifiedProperties();
    }

    private static void SetupClueNotes(Material matPaper)
    {
        SetupEmilyClueInDrawerSO(matPaper);

        CreateClueNoteSO("Clue_Note_Library", new Vector3(-9.7f, 0.79f, -8.5f), "clue_library_occult",
            "Mẩu giấy cổ thư Thư Viện - Nghi thức Cõi Gương:\n\n" +
            "\"Để mở phong ấn cõi bóng tối, hãy nhớ thứ tự cổ xưa:\n" +
            "- 4 bức tường đá giam giữ các linh hồn lang thang (4)\n" +
            "- 2 chiếc bóng đối diện nhau trong chiếc gương ma (2)\n" +
            "- 1 con dao hiến tế nhuốm máu (1)\n" +
            "- 9 tầng luân hồi cõi vĩnh hằng (9)\"\n\n" +
            "[MANH MỐI: Ghép 4 con số theo thứ tự nghi thức -> 4219]",
            "Đọc cổ thư Thư Viện [E / Chạm]", matPaper);

        CreateClueNoteSO("Clue_Note_Storage", new Vector3(9.8f, 0.80f, 8.5f), "clue_storage_blood",
            "Mẩu giấy dính máu dưới chân bàn ăn:\n\n" +
            "\"Gã thợ khóa nhà kho đã khắc các vết cào lên vách gỗ trước khi mất tích:\n" +
            "NĂM vệt cào ở gian ngoài kho chứa (5)\n" +
            "BẢY giọt máu khô trên phiến đá lạnh (7)\n" +
            "BA tiếng gõ cửa lúc nửa đêm (3)\n" +
            "MỘT con dao nghi thức bị niêm phong (1)...\"\n\n" +
            "[MANH MỐI: Mật mã két sắt nhà kho là 5731]",
            "Đọc mẩu giấy dính máu [E / Chạm]", matPaper);
    }

    private static void SetupEmilyClueInDrawerSO(Material matPaper)
    {
        string name = "Clue_Note_Emily";
        GameObject clueObj = GameObject.Find(name);
        if (clueObj == null)
        {
            clueObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clueObj.name = name;
            Undo.RegisterCreatedObjectUndo(clueObj, "Create " + name);
        }
        else
        {
            Undo.RecordObject(clueObj.transform, "Move " + name);
        }

        // Tìm bàn phòng ngủ và ngăn kéo Draw 1_low
        GameObject deskObj = GameObject.Find("Prop_Bedroom_Desk");
        Transform targetDrawer = null;

        if (deskObj != null)
        {
            Transform[] children = deskObj.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in children)
            {
                if (t.name == "Draw 1_low" || t.name.Equals("Draw 1_low", System.StringComparison.OrdinalIgnoreCase))
                {
                    targetDrawer = t;
                    break;
                }
            }

            if (targetDrawer == null)
            {
                DrawerInteractive di = deskObj.GetComponentInChildren<DrawerInteractive>(true);
                if (di != null) targetDrawer = di.transform;
            }
        }

        Vector3 noteSize = new Vector3(0.24f, 0.012f, 0.18f);

        if (targetDrawer != null)
        {
            Renderer drawerRend = targetDrawer.GetComponent<Renderer>();
            Vector3 drawerCenter = drawerRend != null ? drawerRend.bounds.center : targetDrawer.position;
            Vector3 worldPos = drawerCenter + new Vector3(0f, 0.02f, 0f);

            clueObj.transform.position = worldPos;
            clueObj.transform.rotation = targetDrawer.rotation;
            clueObj.transform.SetParent(targetDrawer, true);

            Vector3 parentLossy = targetDrawer.lossyScale;
            clueObj.transform.localScale = new Vector3(
                noteSize.x / Mathf.Max(0.001f, parentLossy.x),
                noteSize.y / Mathf.Max(0.001f, parentLossy.y),
                noteSize.z / Mathf.Max(0.001f, parentLossy.z)
            );

            // Cấu hình ngăn kéo: đảm bảo Collider cứng và hướng trượt chuẩn
            DrawerInteractive di = targetDrawer.GetComponent<DrawerInteractive>();
            if (di != null)
            {
                SerializedObject diSO = new SerializedObject(di);
                diSO.FindProperty("slideDirection").vector3Value = new Vector3(0, 0, 1);
                diSO.FindProperty("slideDistance").floatValue = 0.35f;
                diSO.ApplyModifiedProperties();
            }

            BoxCollider drawerCol = targetDrawer.GetComponent<BoxCollider>();
            if (drawerCol != null && drawerCol.isTrigger)
            {
                drawerCol.isTrigger = false;
            }

            Debug.Log($"[MansionEditor] Đã gắn '{name}' vào bên trong ngăn kéo '{targetDrawer.name}'!");
        }
        else
        {
            clueObj.transform.position = new Vector3(-6.7f, 0.65f, 14.5f);
            clueObj.transform.localScale = noteSize;
        }

        if (matPaper != null)
        {
            MeshRenderer mr = clueObj.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = matPaper;
        }

        CodeClue clueComp = clueObj.GetComponent<CodeClue>();
        if (clueComp == null) clueComp = clueObj.AddComponent<CodeClue>();

        SerializedObject so = new SerializedObject(clueComp);
        so.FindProperty("clueId").stringValue = "clue_emily_diary";
        so.FindProperty("clueText").stringValue =
            "Trang nhật ký của Emily giấu kín trong ngăn kéo bàn:\n\n" +
            "\"Cha mang về chiếc gương cổ bằng vàng từ một phiên đấu giá bí mật. " +
            "Cha bảo nó được đúc vào đúng NĂM 1894 - năm khởi đầu cho dòng họ chúng ta.\n\n" +
            "Từ đêm hôm đó, cha luôn nhốt mình trong phòng và nói chuyện một mình trước tấm gương...\"\n\n" +
            "[MANH MỐI THEN CHỐT: Mật mã két sắt phòng ngủ là 1894]";
        so.FindProperty("interactionPrompt").stringValue = "Đọc nhật ký của Emily trong ngăn kéo [E / Chạm]";
        so.FindProperty("interactionRange").floatValue = 2.5f;
        so.ApplyModifiedProperties();
    }

    private static void CreateClueNoteSO(string name, Vector3 pos, string clueId, string text, string prompt, Material mat)
    {
        GameObject clueObj = GameObject.Find(name);
        if (clueObj == null)
        {
            clueObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clueObj.name = name;
            Undo.RegisterCreatedObjectUndo(clueObj, "Create " + name);
        }

        clueObj.transform.position = pos;
        clueObj.transform.localScale = new Vector3(0.32f, 0.012f, 0.24f);

        if (mat != null)
        {
            MeshRenderer mr = clueObj.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = mat;
        }

        CodeClue clueComp = clueObj.GetComponent<CodeClue>();
        if (clueComp == null) clueComp = clueObj.AddComponent<CodeClue>();

        SerializedObject so = new SerializedObject(clueComp);
        so.FindProperty("clueId").stringValue = clueId;
        so.FindProperty("clueText").stringValue = text;
        so.FindProperty("interactionPrompt").stringValue = prompt;
        so.FindProperty("interactionRange").floatValue = 2.5f;
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateOrReplaceBox(string name, Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject box = GameObject.Find(name);
        if (box == null)
        {
            box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            Undo.RegisterCreatedObjectUndo(box, "Create " + name);
        }

        box.layer = 3;
        box.transform.position = pos;
        box.transform.localScale = scale;

        if (mat != null)
        {
            MeshRenderer mr = box.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = mat;
        }

        return box;
    }

    private static GameObject CreateFallbackDoor(string name, Vector3 pos, Quaternion rot)
    {
        GameObject doorObj = new GameObject(name);
        doorObj.transform.position = pos;
        doorObj.transform.rotation = rot;
        Undo.RegisterCreatedObjectUndo(doorObj, "Create Fallback " + name);

        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = "Door_WoodPanel";
        panel.transform.SetParent(doorObj.transform, false);
        panel.transform.localPosition = new Vector3(0f, 1.05f, 0f);
        panel.transform.localScale = new Vector3(1.05f, 2.1f, 0.08f);

        BoxCollider col = doorObj.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.05f, 0f);
        col.size = new Vector3(1.1f, 2.1f, 0.15f);

        return doorObj;
    }
}
#endif
