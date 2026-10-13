using UnityEngine;
using System.Reflection;

/// <summary>
/// MANSION PUZZLE & DOORS MANAGER
/// Tự động thiết lập và quản lý hệ thống:
/// 1. 3 Manh mối kinh dị (Clue Notes) trên bàn của các phòng.
/// 2. 3 Két sắt mật mã (Safes) với code chuẩn, phần thưởng chìa khóa, và prompt không bị lộ code.
/// 3. 2 Cửa thông phòng mở lối đi trực tiếp:
///    - Phòng ngủ Emily <-> Thư viện (West Rooms)
///    - Phòng ăn <-> Nhà kho (East Rooms)
/// </summary>
public class MansionPuzzleAndDoorsManager : MonoBehaviour
{
    private const string WestDoorName = "Door_Bedroom_Library";
    private const string EastDoorName = "Door_Dining_Storage";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Level_Main" || sceneName == "World2_Prototype")
        {
            EnsureManagerExists();
        }
    }

    public static MansionPuzzleAndDoorsManager EnsureManagerExists()
    {
        MansionPuzzleAndDoorsManager existing = FindAnyObjectByType<MansionPuzzleAndDoorsManager>();
        if (existing != null)
        {
            return existing;
        }

        GameObject go = new GameObject("--- MANSION_PUZZLE_AND_DOORS_MANAGER ---");
        return go.AddComponent<MansionPuzzleAndDoorsManager>();
    }

    private void Awake()
    {
        SetupSafes();
        SetupClues();
        SetupConnectingDoors();
    }

    // =========================================================================
    // 1. SETUP SAFES & PASSCODES
    // =========================================================================
    public void SetupSafes()
    {
        // Safe 1: Phòng ngủ Emily (Mã: 1894 -> Thưởng: key_library)
        ConfigureSafe("Safe_Bedroom", "safe_bedroom", "1894", "key_library", 
            "KÉT SẮT PHÒNG NGỦ", "Két Sắt Cổ (Nhấn E để nhập mật mã)");

        // Safe 2: Thư viện (Mã: 4219 -> Thưởng: key_storage)
        ConfigureSafe("Safe_Library", "safe_library", "4219", "key_storage", 
            "KÉT BÍ MẬT THƯ VIỆN", "Két Cổ Thư Viện (Nhấn E để nhập mật mã)");

        // Safe 3: Nhà kho (Mã: 5731 -> Thưởng: key_ritual_dagger)
        ConfigureSafe("Safe_Storage_Dagger", "chest_storage", "5731", "key_ritual_dagger", 
            "KÉT KIM LOẠI NHÀ KHO", "Két Kim Loại Nhà Kho (Nhấn E để nhập mật mã)");
    }

    private void ConfigureSafe(string gameObjectName, string lockId, string code, string rewardKey, string displayName, string prompt)
    {
        GameObject safeObj = GameObject.Find(gameObjectName);
        if (safeObj == null) return;

        Lockable lockable = safeObj.GetComponent<Lockable>();
        if (lockable == null) lockable = safeObj.AddComponent<Lockable>();

        SetPrivateField(lockable, "lockType", LockType.Code);
        SetPrivateField(lockable, "lockId", lockId);
        SetPrivateField(lockable, "correctCode", code);
        SetPrivateField(lockable, "rewardKeyId", rewardKey);
        SetPrivateField(lockable, "lockDisplayName", displayName);
        SetPrivateField(lockable, "interactionPrompt", prompt);
        SetPrivateField(lockable, "interactionRange", 2.8f);

        Debug.Log($"[MansionManager] Configured Safe '{gameObjectName}' -> ID: {lockId}, Code: {code}, Reward: {rewardKey}");
    }

    // =========================================================================
    // 2. SETUP CLUE NOTES
    // =========================================================================
    public void SetupClues()
    {
        Material paperMat = Resources.Load<Material>("Mat_Clue_Paper");
        if (paperMat == null)
        {
            paperMat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            paperMat.color = new Color(0.92f, 0.88f, 0.74f);
        }

        // Clue 1: Phòng ngủ Emily (trên bàn ngủ gần két)
        CreateOrUpdateClue(
            "Clue_Note_Emily",
            new Vector3(-7.7f, 0.79f, 14.5f),
            new Vector3(0.32f, 0.012f, 0.24f),
            "clue_emily_diary",
            "Trang nhật ký của Emily - Ngày 14 tháng 10:\n\n" +
            "\"Cha mang về chiếc gương cổ bằng vàng từ một phiên đấu giá bí mật. " +
            "Cha bảo nó được đúc vào đúng NĂM 1894 - năm khởi đầu cho dòng họ chúng ta.\n\n" +
            "Từ đêm hôm đó, cha luôn nhốt mình trong phòng và nói chuyện một mình trước tấm gương...\"\n\n" +
            "[MANH MỐI: Mật mã két sắt phòng ngủ là 1894]",
            "Đọc nhật ký của Emily [E / Chạm]",
            paperMat
        );

        // Clue 2: Thư viện (trên bàn học / đọc sách)
        CreateOrUpdateClue(
            "Clue_Note_Library",
            new Vector3(-9.7f, 0.79f, -8.5f),
            new Vector3(0.32f, 0.012f, 0.24f),
            "clue_library_occult",
            "Mẩu giấy cổ thư Thư Viện - Nghi thức Cõi Gương:\n\n" +
            "\"Để mở phong ấn cõi bóng tối, hãy nhớ thứ tự cổ xưa:\n" +
            "- 4 bức tường đá giam giữ các linh hồn lang thang (4)\n" +
            "- 2 chiếc bóng đối diện nhau trong chiếc gương ma (2)\n" +
            "- 1 con dao hiến tế nhuốm máu (1)\n" +
            "- 9 tầng luân hồi cõi vĩnh hằng (9)\"\n\n" +
            "[MANH MỐI: Ghép 4 con số theo thứ tự nghi thức -> 4219]",
            "Đọc cổ thư Thư Viện [E / Chạm]",
            paperMat
        );

        // Clue 3: Bàn ăn / Nhà kho (mẩu giấy dính máu dưới bàn ăn)
        CreateOrUpdateClue(
            "Clue_Note_Storage",
            new Vector3(9.8f, 0.80f, 8.5f),
            new Vector3(0.32f, 0.012f, 0.24f),
            "clue_storage_blood",
            "Mẩu giấy dính máu dưới chân bàn ăn:\n\n" +
            "\"Gã thợ khóa nhà kho đã khắc các vết cào lên vách gỗ trước khi mất tích:\n" +
            "NĂM vệt cào ở gian ngoài kho chứa (5)\n" +
            "BẢY giọt máu khô trên phiến đá lạnh (7)\n" +
            "BA tiếng gõ cửa lúc nửa đêm (3)\n" +
            "MỘT con dao nghi thức bị niêm phong (1)...\"\n\n" +
            "[MANH MỐI: Mật mã két sắt nhà kho là 5731]",
            "Đọc mẩu giấy dính máu [E / Chạm]",
            paperMat
        );
    }

    private void CreateOrUpdateClue(string name, Vector3 pos, Vector3 scale, string clueId, string text, string prompt, Material mat)
    {
        GameObject clueObj = GameObject.Find(name);
        if (clueObj == null)
        {
            clueObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clueObj.name = name;
            clueObj.transform.position = pos;
            clueObj.transform.localScale = scale;
            if (mat != null)
            {
                MeshRenderer mr = clueObj.GetComponent<MeshRenderer>();
                if (mr != null) mr.sharedMaterial = mat;
            }
        }
        else
        {
            clueObj.transform.position = pos;
            clueObj.transform.localScale = scale;
        }

        // Ensure collider
        Collider col = clueObj.GetComponent<Collider>();
        if (col == null) clueObj.AddComponent<BoxCollider>();

        // Ensure CodeClue component
        CodeClue clueComp = clueObj.GetComponent<CodeClue>();
        if (clueComp == null) clueComp = clueObj.AddComponent<CodeClue>();

        clueComp.SetClueData(clueId, text, prompt);

        Debug.Log($"[MansionManager] Configured Clue '{name}' at {pos}");
    }

    // =========================================================================
    // 3. SETUP CONNECTING DOORS BETWEEN ROOMS
    // =========================================================================
    public void SetupConnectingDoors()
    {
        Material wallMat = Resources.Load<Material>("Mat_Wall_Dark");
        if (wallMat == null)
        {
            GameObject anyWall = GameObject.Find("Wall_Outer_North");
            if (anyWall != null && anyWall.GetComponent<MeshRenderer>() != null)
                wallMat = anyWall.GetComponent<MeshRenderer>().sharedMaterial;
        }

        float H = 3.5f;
        float T = 0.35f;

        // --- CỬA 1: PHÒNG NGỦ EMILY <-> THƯ VIỆN (West Rooms, Z = 0) ---
        SetupWestConnectingDoor(H, T, wallMat);

        // --- CỬA 2: PHÒNG ĂN <-> NHÀ KHO (East Rooms, Z = 0) ---
        SetupEastConnectingDoor(H, T, wallMat);
    }

    private void SetupWestConnectingDoor(float H, float T, Material wallMat)
    {
        GameObject solidWall = GameObject.Find("Wall_Divider_West_Rooms");
        if (solidWall != null)
        {
            solidWall.SetActive(false); // Ẩn bức tường kín cũ
        }

        // Tạo 2 đoạn tường mới chừa khoảng trống cửa tại X = -9.5f, rộng 1.2m
        EnsureBox("Wall_West_Divider_Inner", new Vector3(-5.7f, H / 2, 0f), new Vector3(6.4f, H, T), wallMat);
        EnsureBox("Wall_West_Divider_Outer", new Vector3(-13.55f, H / 2, 0f), new Vector3(6.9f, H, T), wallMat);
        EnsureBox("Lintel_Bedroom_Library_Door", new Vector3(-9.5f, 2.85f, 0f), new Vector3(1.2f, 1.3f, T), wallMat);

        // Tạo cánh cửa tương tác tại X = -9.5f, Y = 0, Z = 0
        GameObject doorObj = GameObject.Find(WestDoorName);
        if (doorObj == null)
        {
            doorObj = CreateDoorObject(WestDoorName, new Vector3(-9.5f, 0f, 0f), Quaternion.identity);
        }

        DoorInteractive doorComp = doorObj.GetComponent<DoorInteractive>();
        if (doorComp == null) doorComp = doorObj.AddComponent<DoorInteractive>();
        doorComp.SetLocked(false);
    }

    private void SetupEastConnectingDoor(float H, float T, Material wallMat)
    {
        GameObject solidWall = GameObject.Find("Wall_Divider_East_Rooms");
        if (solidWall != null)
        {
            solidWall.SetActive(false); // Ẩn bức tường kín cũ
        }

        // Tạo 2 đoạn tường mới chừa khoảng trống cửa tại X = +9.5f, rộng 1.2m
        EnsureBox("Wall_East_Divider_Inner", new Vector3(5.7f, H / 2, 0f), new Vector3(6.4f, H, T), wallMat);
        EnsureBox("Wall_East_Divider_Outer", new Vector3(13.55f, H / 2, 0f), new Vector3(6.9f, H, T), wallMat);
        EnsureBox("Lintel_Dining_Storage_Door", new Vector3(9.5f, 2.85f, 0f), new Vector3(1.2f, 1.3f, T), wallMat);

        // Tạo cánh cửa tương tác tại X = +9.5f, Y = 0, Z = 0
        GameObject doorObj = GameObject.Find(EastDoorName);
        if (doorObj == null)
        {
            doorObj = CreateDoorObject(EastDoorName, new Vector3(9.5f, 0f, 0f), Quaternion.identity);
        }

        DoorInteractive doorComp = doorObj.GetComponent<DoorInteractive>();
        if (doorComp == null) doorComp = doorObj.AddComponent<DoorInteractive>();
        doorComp.SetLocked(false);
    }

    private GameObject EnsureBox(string name, Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject box = GameObject.Find(name);
        if (box == null)
        {
            box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
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

    private GameObject CreateDoorObject(string name, Vector3 pos, Quaternion rot)
    {
        GameObject doorPrefab = Resources.Load<GameObject>("Victorian_Door");
        GameObject doorObj;

        if (doorPrefab != null)
        {
            doorObj = Instantiate(doorPrefab, pos, rot);
            doorObj.name = name;
        }
        else
        {
            // Procedural Victorian Door nếu không load được prefab
            doorObj = new GameObject(name);
            doorObj.transform.position = pos;
            doorObj.transform.rotation = rot;

            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "Door_WoodPanel";
            panel.transform.SetParent(doorObj.transform, false);
            panel.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            panel.transform.localScale = new Vector3(1.05f, 2.1f, 0.08f);

            // Door handle
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Door_Handle";
            handle.transform.SetParent(panel.transform, false);
            handle.transform.localPosition = new Vector3(0.4f, 0f, -0.06f);
            handle.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            handle.transform.localScale = new Vector3(0.06f, 0.08f, 0.06f);
            Collider handleCol = handle.GetComponent<Collider>();
            if (handleCol != null) Destroy(handleCol);

            BoxCollider doorCol = doorObj.AddComponent<BoxCollider>();
            doorCol.center = new Vector3(0f, 1.05f, 0f);
            doorCol.size = new Vector3(1.1f, 2.1f, 0.15f);

            Material doorMat = Resources.Load<Material>("Mat_Floor_Wood");
            if (doorMat != null) panel.GetComponent<MeshRenderer>().sharedMaterial = doorMat;
        }

        return doorObj;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        if (target == null) return;
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }
}
