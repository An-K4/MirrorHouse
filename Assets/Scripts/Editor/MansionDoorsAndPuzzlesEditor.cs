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
public class MansionDoorsAndPuzzlesEditor : EditorWindow
{
    [MenuItem("MirrorHouse/Setup Connecting Doors & Horror Clues (Permanent)", false, 20)]
    public static void SetupDoorsAndPuzzles()
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main")
        {
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
                    EditorSceneManager.OpenScene(scenePath);
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

        // Đánh dấu Scene Dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        string summary = "✅ THIẾT LẬP HOÀN TẤT VÀ ĐÃ BAKE VÀO SCENE!\n\n" +
            "🚪 2 CỬA THÔNG PHÒNG MỚI:\n" +
            "  1. Door_Bedroom_Library: Phòng ngủ Emily <-> Thư viện (X: -9.5, Z: 0)\n" +
            "  2. Door_Dining_Storage: Phòng ăn <-> Nhà kho (X: 9.5, Z: 0)\n\n" +
            "🔐 3 KÉT SẮT & MẬT MÃ:\n" +
            "  1. Két phòng ngủ: Mã 1894 (Thưởng chìa thư viện)\n" +
            "  2. Két thư viện: Mã 4219 (Thưởng chìa nhà kho)\n" +
            "  3. Két nhà kho: Mã 5731 (Thưởng Dao tế Mirror Dagger)\n\n" +
            "📜 3 MANH MỐI KINH DỊ:\n" +
            "  1. Clue_Note_Emily: Trên bàn phòng ngủ (Nhật ký năm 1894)\n" +
            "  2. Clue_Note_Library: Trên bàn thư viện (Cổ thư 4219)\n" +
            "  3. Clue_Note_Storage: Dưới chân bàn ăn (Vệt cào máu 5731)\n\n" +
            "👉 Nhấn Ctrl + S để lưu scene!";

        Debug.Log($"[MansionDoorsAndPuzzlesEditor]\n{summary}");
        EditorUtility.DisplayDialog("Mirror House - Setup Hoàn Tất", summary, "Tuyệt vời!");
    }

    private static void SetupWestDoor(float H, float T, Material matWall, GameObject doorPrefab)
    {
        // Xóa hoặc ẩn bức tường kín cũ
        GameObject oldSolidWall = GameObject.Find("Wall_Divider_West_Rooms");
        if (oldSolidWall != null)
        {
            Undo.DestroyObjectImmediate(oldSolidWall);
        }

        // Tạo 2 mảng tường hai bên cửa và lanh-tô phía trên
        CreateOrReplaceBox("Wall_West_Divider_Inner", new Vector3(-5.7f, H / 2, 0f), new Vector3(6.4f, H, T), matWall);
        CreateOrReplaceBox("Wall_West_Divider_Outer", new Vector3(-13.55f, H / 2, 0f), new Vector3(6.9f, H, T), matWall);
        CreateOrReplaceBox("Lintel_Bedroom_Library_Door", new Vector3(-9.5f, 2.85f, 0f), new Vector3(1.2f, 1.3f, T), matWall);

        // Cửa thông phòng
        GameObject doorObj = GameObject.Find("Door_Bedroom_Library");
        if (doorObj == null)
        {
            if (doorPrefab != null)
            {
                doorObj = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorObj.name = "Door_Bedroom_Library";
                doorObj.transform.position = new Vector3(-9.5f, 0f, 0f);
                doorObj.transform.rotation = Quaternion.identity;
                Undo.RegisterCreatedObjectUndo(doorObj, "Create Door_Bedroom_Library");
            }
            else
            {
                doorObj = CreateFallbackDoor("Door_Bedroom_Library", new Vector3(-9.5f, 0f, 0f), Quaternion.identity);
            }
        }

        DoorInteractive doorComp = doorObj.GetComponent<DoorInteractive>();
        if (doorComp == null) doorComp = doorObj.AddComponent<DoorInteractive>();
        doorComp.SetLocked(false);
    }

    private static void SetupEastDoor(float H, float T, Material matWall, GameObject doorPrefab)
    {
        // Xóa bức tường kín cũ
        GameObject oldSolidWall = GameObject.Find("Wall_Divider_East_Rooms");
        if (oldSolidWall != null)
        {
            Undo.DestroyObjectImmediate(oldSolidWall);
        }

        // Tạo 2 mảng tường hai bên cửa và lanh-tô phía trên
        CreateOrReplaceBox("Wall_East_Divider_Inner", new Vector3(5.7f, H / 2, 0f), new Vector3(6.4f, H, T), matWall);
        CreateOrReplaceBox("Wall_East_Divider_Outer", new Vector3(13.55f, H / 2, 0f), new Vector3(6.9f, H, T), matWall);
        CreateOrReplaceBox("Lintel_Dining_Storage_Door", new Vector3(9.5f, 2.85f, 0f), new Vector3(1.2f, 1.3f, T), matWall);

        // Cửa thông phòng
        GameObject doorObj = GameObject.Find("Door_Dining_Storage");
        if (doorObj == null)
        {
            if (doorPrefab != null)
            {
                doorObj = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorObj.name = "Door_Dining_Storage";
                doorObj.transform.position = new Vector3(9.5f, 0f, 0f);
                doorObj.transform.rotation = Quaternion.identity;
                Undo.RegisterCreatedObjectUndo(doorObj, "Create Door_Dining_Storage");
            }
            else
            {
                doorObj = CreateFallbackDoor("Door_Dining_Storage", new Vector3(9.5f, 0f, 0f), Quaternion.identity);
            }
        }

        DoorInteractive doorComp = doorObj.GetComponent<DoorInteractive>();
        if (doorComp == null) doorComp = doorObj.AddComponent<DoorInteractive>();
        doorComp.SetLocked(false);
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
        CreateClueNoteSO("Clue_Note_Emily", new Vector3(-7.7f, 0.79f, 14.5f), "clue_emily_diary",
            "Trang nhật ký của Emily - Ngày 14 tháng 10:\n\n" +
            "\"Cha mang về chiếc gương cổ bằng vàng từ một phiên đấu giá bí mật. " +
            "Cha bảo nó được đúc vào đúng NĂM 1894 - năm khởi đầu cho dòng họ chúng ta.\n\n" +
            "Từ đêm hôm đó, cha luôn nhốt mình trong phòng và nói chuyện một mình trước tấm gương...\"\n\n" +
            "[MANH MỐI: Mật mã két sắt phòng ngủ là 1894]",
            "Đọc nhật ký của Emily [E / Chạm]", matPaper);

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
