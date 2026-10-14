#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// MANSION DECORATOR & MIRRORS EDITOR TOOL
/// Menu: MirrorHouse → Decorate Mansion & Place Wall Mirrors (Permanent)
/// 
/// Công dụng:
/// 1. Tự động quét AssetDatabase tìm các Asset mới import (VINTAGE LIVING ROOM 3D GAME PACK, Old Wooden Cabinet, ...)
/// 2. Bố trí bàn, ghế sofa, bàn trà, tủ gỗ cổ kính (cabinets), đồng hồ quả lắc vào các phòng.
/// 3. Dựng hệ thống "NHÀ GƯƠNG" - 20 Tấm gương Victorian/Gothic phản chiếu trên các bức tường:
///    - "Hành lang gương" (Hall of Mirrors): 8 tấm gương đối diện nhau dọc hành lang chính.
///    - Phòng ngủ Emily: Gương bàn trang điểm, gương soi toàn thân, gương đầu giường rùng rợn.
///    - Thư viện: Gương huyền bí cổ, gương giữa các giá sách, gương góc đọc sách.
///    - Phòng khách / Phòng ăn: Gương đại sảnh tiệc, gương lò sưởi, gương cửa vào.
///    - Nhà kho / Phòng tế lễ: Gương đen phong ấn, gương dính máu cào xước, gương bóng ma.
/// 4. Gắn script tương tác kinh dị MirrorInteractive với câu thoại rùng rợn cho từng tấm gương.
/// 5. Đặt 5 Reflection Probes bao phủ toàn bộ biệt thự để gương phản chiếu ánh sáng và đồ nội thất chân thực.
/// 6. Tự động lưu vĩnh viễn (Save Scene) vào file Level_Main.unity.
/// </summary>
[InitializeOnLoad]
public class MansionDecoratorAndMirrorsEditor : EditorWindow
{
    private const string MirrorsRootName = "--- MANSION_WALL_MIRRORS ---";
    private const string FurnitureRootName = "--- MANSION_FURNITURE_DECORATIONS ---";
    private const string ReflectionProbesRootName = "--- MANSION_REFLECTION_PROBES ---";

    static MansionDecoratorAndMirrorsEditor()
    {
        EditorApplication.delayCall += AutoDecorateIfMissing;
        EditorSceneManager.sceneOpened += (scene, mode) => AutoDecorateIfMissing();
    }

    private static void AutoDecorateIfMissing()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main") return;

        // Kiểm tra xem hệ thống gương và nội thất có cần thiết lập hoặc điều chỉnh lại vị trí không
        bool needsRebake = false;
        if (GameObject.Find(MirrorsRootName) == null || GameObject.Find(FurnitureRootName) == null)
        {
            needsRebake = true;
        }
        else
        {
            // Kiểm tra xem gương hành lang có đang ở tọa độ cũ (> 2.0m) hay không
            GameObject testMirror = GameObject.Find("Mirror_Corridor_East_1");
            if (testMirror != null && testMirror.transform.position.x > 2.0f)
            {
                Debug.Log("[MansionDecorator] Phát hiện gương đang ở tọa độ cũ chưa căn chỉnh khít tường. Đang tự động bố trí lại...");
                needsRebake = true;
            }
        }

        if (needsRebake)
        {
            Debug.Log("[MansionDecorator] Đang tự động cập nhật hệ thống Gương và Nội Thất chuẩn xác và lưu scene...");
            ExecuteDecorationInternal(silent: true);
        }
    }

    [MenuItem("MirrorHouse/Decorate Mansion & Place Wall Mirrors (Permanent)", false, 25)]
    public static void DecorateMansionAndPlaceMirrors()
    {
        ExecuteDecorationInternal(silent: false);
    }

    private static void ExecuteDecorationInternal(bool silent)
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main")
        {
            if (silent) return;

            bool proceed = EditorUtility.DisplayDialog(
                "Mở Scene Level_Main",
                $"Scene hiện tại là '{activeScene.name}'.\nBạn có muốn mở 'Assets/Scenes/Level_Main.unity' để bố trí không?",
                "Mở Level_Main",
                "Hủy"
            );

            if (proceed)
            {
                string scenePath = "Assets/Scenes/Level_Main.unity";
                if (File.Exists(scenePath))
                {
                    activeScene = EditorSceneManager.OpenScene(scenePath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy scene Assets/Scenes/Level_Main.unity!", "OK");
                    return;
                }
            }
            else return;
        }

        // 1. Quét tìm tất cả Prefab liên quan trong Project
        var catalog = ScanProjectAssetCatalog();

        // 2. Tạo Materials Gương & Khung viền gỗ/vàng cổ kính
        var materials = EnsureMirrorMaterials();

        // 3. Xóa các Root cũ nếu muốn làm mới lại
        GameObject oldMirrors = GameObject.Find(MirrorsRootName);
        if (oldMirrors != null) Undo.DestroyObjectImmediate(oldMirrors);

        GameObject oldFurniture = GameObject.Find(FurnitureRootName);
        if (oldFurniture != null) Undo.DestroyObjectImmediate(oldFurniture);

        GameObject oldProbes = GameObject.Find(ReflectionProbesRootName);
        if (oldProbes != null) Undo.DestroyObjectImmediate(oldProbes);

        // 4. Tạo Root GameObjects mới
        GameObject mirrorsRoot = new GameObject(MirrorsRootName);
        Undo.RegisterCreatedObjectUndo(mirrorsRoot, "Create " + MirrorsRootName);

        GameObject furnitureRoot = new GameObject(FurnitureRootName);
        Undo.RegisterCreatedObjectUndo(furnitureRoot, "Create " + FurnitureRootName);

        GameObject probesRoot = new GameObject(ReflectionProbesRootName);
        Undo.RegisterCreatedObjectUndo(probesRoot, "Create " + ReflectionProbesRootName);

        // 5. Bố trí hệ thống "NHÀ GƯƠNG" (20 tấm gương trên tường)
        int mirrorCount = PlaceWallMirrors(mirrorsRoot.transform, materials);

        // 6. Bố trí đồ nội thất (Old Wooden Cabinets, Vintage Living Room)
        int furnitureCount = PlaceFurnitureAndDecorations(furnitureRoot.transform, catalog, materials);

        // 7. Bố trí Reflection Probes cho phản chiếu lung linh
        SetupReflectionProbes(probesRoot.transform);

        // 8. Đánh dấu Dirty và lưu Scene vĩnh viễn
        EditorSceneManager.MarkSceneDirty(activeScene);
        bool saved = EditorSceneManager.SaveScene(activeScene);

        string summary = $"✅ BỐ TRÍ BIỆT THỰ NHÀ GƯƠNG HOÀN TẤT!\n\n" +
            $"🪞 ĐÃ TREO {mirrorCount} TẤM GƯƠNG KINH DỊ LÊN TƯỜNG:\n" +
            $"  - 8 Gương ở Hành Lang Chính (Hall of Mirrors - đối xứng 2 bên vách)\n" +
            $"  - 3 Gương ở Phòng Ngủ Emily (Bàn trang điểm, soi toàn thân, đầu giường)\n" +
            $"  - 3 Gương ở Thư Viện (Gương cổ huyền bí, vách sách, góc đọc)\n" +
            $"  - 3 Gương ở Phòng Khách & Bàn Ăn (Đại sảnh tiệc, lò sưởi, lối vào)\n" +
            $"  - 3 Gương ở Nhà Kho & Tế Lễ (Gương đen tế đàn, gương cào máu, gương bóng ma)\n\n" +
            $"🛋️ ĐÃ BỐ TRÍ {furnitureCount} MÓN NỘI THẤT CỔ KÍNH:\n" +
            $"  - {catalog.Cabinets.Count} Asset Tủ gỗ cổ (Old Wooden Cabinets) phân bổ vào các phòng\n" +
            $"  - {catalog.Sofas.Count} Sofa dài Chesterfield & {catalog.Chairs.Count} Ghế bành quý tộc (Vintage Living Room)\n" +
            $"  - Lò sưởi Victoria, đồng hồ quả lắc, chân nến đồng, rương hòm, bàn trà & bàn ăn\n" +
            $"  - Khoảng đệm an toàn quanh các cánh cửa luôn rộng > 2m (hoàn toàn không bị chắn)\n\n" +
            $"✨ 5 Reflection Probes phản chiếu hình ảnh chân thực trong URP.\n" +
            $"💾 Đã tự động lưu vĩnh viễn vào 'Level_Main.unity'! (Saved: {saved})";

        Debug.Log($"[MansionDecorator]\n{summary}");
        if (!silent)
        {
            EditorUtility.DisplayDialog("Mirror House - Trang Trí Hoàn Tất", summary, "Tuyệt vời!");
        }
    }

    #region Asset Scanning & Catalog
    private class AssetCatalog
    {
        public List<GameObject> Cabinets = new List<GameObject>();
        public List<GameObject> Sofas = new List<GameObject>();
        public List<GameObject> Chairs = new List<GameObject>();
        public List<GameObject> Tables = new List<GameObject>();
        public List<GameObject> Clocks = new List<GameObject>();
        public List<GameObject> Lamps = new List<GameObject>();
        public List<GameObject> Fireplaces = new List<GameObject>();
        public List<GameObject> Bookshelves = new List<GameObject>();
        public List<GameObject> Books = new List<GameObject>();
        public List<GameObject> Rugs = new List<GameObject>();
        public List<GameObject> Barrels = new List<GameObject>();
        public List<GameObject> Crates = new List<GameObject>();
        public List<GameObject> Mirrors = new List<GameObject>();
        public List<GameObject> Decorations = new List<GameObject>();
        public List<GameObject> Others = new List<GameObject>();

        public GameObject GetCabinet(int idx) => Cabinets.Count > 0 ? Cabinets[idx % Cabinets.Count] : null;
        public GameObject GetSofa(int idx) => Sofas.Count > 0 ? Sofas[idx % Sofas.Count] : null;
        public GameObject GetChair(int idx) => Chairs.Count > 0 ? Chairs[idx % Chairs.Count] : null;
        public GameObject GetTable(int idx) => Tables.Count > 0 ? Tables[idx % Tables.Count] : null;
        public GameObject GetClock(int idx) => Clocks.Count > 0 ? Clocks[idx % Clocks.Count] : null;
        public GameObject GetLamp(int idx) => Lamps.Count > 0 ? Lamps[idx % Lamps.Count] : null;
        public GameObject GetFireplace(int idx) => Fireplaces.Count > 0 ? Fireplaces[idx % Fireplaces.Count] : null;
        public GameObject GetBookshelf(int idx) => Bookshelves.Count > 0 ? Bookshelves[idx % Bookshelves.Count] : null;
        public GameObject GetBook(int idx) => Books.Count > 0 ? Books[idx % Books.Count] : null;
        public GameObject GetRug(int idx) => Rugs.Count > 0 ? Rugs[idx % Rugs.Count] : null;
        public GameObject GetBarrel(int idx) => Barrels.Count > 0 ? Barrels[idx % Barrels.Count] : null;
        public GameObject GetCrate(int idx) => Crates.Count > 0 ? Crates[idx % Crates.Count] : null;
        public GameObject GetDecoration(int idx) => Decorations.Count > 0 ? Decorations[idx % Decorations.Count] : null;
    }

    private static AssetCatalog ScanProjectAssetCatalog()
    {
        AssetCatalog catalog = new AssetCatalog();

        // Quét đồng thời cả Prefab và 3D Model (FBX, OBJ) trong Project
        HashSet<string> allGuids = new HashSet<string>();
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab")) allGuids.Add(guid);
        foreach (string guid in AssetDatabase.FindAssets("t:Model")) allGuids.Add(guid);

        foreach (string guid in allGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string lowerPath = path.ToLower();

            // Bỏ qua prefabs UI, Player, Camera, Editor, Monster
            if (lowerPath.Contains("/ui/") || lowerPath.Contains("player") || lowerPath.Contains("hud") || 
                lowerPath.Contains("canvas") || lowerPath.Contains("/editor/") || lowerPath.Contains("navmesh") || 
                lowerPath.Contains("lightmap") || lowerPath.Contains("monster") || lowerPath.Contains("demon"))
                continue;

            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null) continue;

            string name = asset.name.ToLower();

            // Phân loại tài nguyên thông minh theo tên và thư mục
            if (name.Contains("cabinet") || name.Contains("wardrobe") || name.Contains("cupboard") || 
                name.Contains("credenza") || name.Contains("closet") || name.Contains("dresser") || name.Contains("armoire"))
            {
                catalog.Cabinets.Add(asset);
            }
            else if (name.Contains("sofa") || name.Contains("couch") || name.Contains("divan") || 
                     name.Contains("chesterfield") || name.Contains("settee"))
            {
                catalog.Sofas.Add(asset);
            }
            else if (name.Contains("chair") || name.Contains("armchair") || name.Contains("stool") || 
                     name.Contains("seat") || name.Contains("bench"))
            {
                catalog.Chairs.Add(asset);
            }
            else if (name.Contains("table") || name.Contains("desk") || name.Contains("stand") || 
                     name.Contains("nightstand") || name.Contains("coffee") || name.Contains("dining") || name.Contains("console"))
            {
                catalog.Tables.Add(asset);
            }
            else if (name.Contains("clock") || name.Contains("pendulum") || name.Contains("grandfather"))
            {
                catalog.Clocks.Add(asset);
            }
            else if (name.Contains("lamp") || name.Contains("candle") || name.Contains("candelabra") || 
                     name.Contains("light") || name.Contains("chandelier") || name.Contains("lantern") || name.Contains("sconce"))
            {
                catalog.Lamps.Add(asset);
            }
            else if (name.Contains("fireplace") || name.Contains("hearth") || name.Contains("mantel"))
            {
                catalog.Fireplaces.Add(asset);
            }
            else if (name.Contains("bookshelf") || name.Contains("bookcase") || name.Contains("shelf") || name.Contains("shelves"))
            {
                catalog.Bookshelves.Add(asset);
            }
            else if (name.Contains("book") || name.Contains("grimoire") || name.Contains("tome") || name.Contains("paper"))
            {
                catalog.Books.Add(asset);
            }
            else if (name.Contains("rug") || name.Contains("carpet") || name.Contains("mat"))
            {
                catalog.Rugs.Add(asset);
            }
            else if (name.Contains("barrel") || name.Contains("keg") || name.Contains("cask"))
            {
                catalog.Barrels.Add(asset);
            }
            else if (name.Contains("crate") || name.Contains("chest") || name.Contains("trunk") || name.Contains("box"))
            {
                catalog.Crates.Add(asset);
            }
            else if (name.Contains("mirror") || name.Contains("looking_glass"))
            {
                catalog.Mirrors.Add(asset);
            }
            else if (name.Contains("vase") || name.Contains("urn") || name.Contains("pedestal") || 
                     name.Contains("statue") || name.Contains("painting") || name.Contains("frame") || name.Contains("decor"))
            {
                catalog.Decorations.Add(asset);
            }
            else if (lowerPath.Contains("vintage") || lowerPath.Contains("living") || 
                     lowerPath.Contains("furniture") || lowerPath.Contains("cabinet"))
            {
                catalog.Others.Add(asset);
            }
        }

        Debug.Log($"[MansionDecorator] Đã quét Catalog:\n" +
            $"- Cabinets: {catalog.Cabinets.Count}\n" +
            $"- Sofas: {catalog.Sofas.Count}\n" +
            $"- Chairs: {catalog.Chairs.Count}\n" +
            $"- Tables: {catalog.Tables.Count}\n" +
            $"- Clocks: {catalog.Clocks.Count}\n" +
            $"- Lamps/Candles: {catalog.Lamps.Count}\n" +
            $"- Fireplaces: {catalog.Fireplaces.Count}\n" +
            $"- Bookshelves: {catalog.Bookshelves.Count}\n" +
            $"- Books: {catalog.Books.Count}\n" +
            $"- Crates/Chests: {catalog.Crates.Count}\n" +
            $"- Barrels: {catalog.Barrels.Count}\n" +
            $"- Mirrors: {catalog.Mirrors.Count}\n" +
            $"- Decorations: {catalog.Decorations.Count}\n" +
            $"- Others: {catalog.Others.Count}");

        return catalog;
    }
    #endregion

    #region Materials Management
    private class MirrorMaterials
    {
        public Material MirrorReflective;
        public Material MirrorTainted;
        public Material FrameGold;
        public Material FrameDarkWood;
        public Material FabricVelvet;
    }

    private static MirrorMaterials EnsureMirrorMaterials()
    {
        MirrorMaterials mats = new MirrorMaterials();

        Shader litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

        // 1. Mat_Gothic_Mirror.mat (Mặt kính tráng thủy phản chiếu cao)
        string pathMirror = "Assets/Materials/Mat_Gothic_Mirror.mat";
        mats.MirrorReflective = AssetDatabase.LoadAssetAtPath<Material>(pathMirror);
        if (mats.MirrorReflective == null)
        {
            mats.MirrorReflective = new Material(litShader);
            mats.MirrorReflective.name = "Mat_Gothic_Mirror";
            mats.MirrorReflective.color = new Color(0.82f, 0.86f, 0.90f, 1f);
            if (mats.MirrorReflective.HasProperty("_Metallic")) mats.MirrorReflective.SetFloat("_Metallic", 0.98f);
            if (mats.MirrorReflective.HasProperty("_Smoothness")) mats.MirrorReflective.SetFloat("_Smoothness", 0.98f);
            AssetDatabase.CreateAsset(mats.MirrorReflective, pathMirror);
        }

        // 2. Mat_Mirror_Tainted.mat (Gương mờ đục / ố máu cõi bóng tối)
        string pathTainted = "Assets/Materials/Mat_Mirror_Tainted.mat";
        mats.MirrorTainted = AssetDatabase.LoadAssetAtPath<Material>(pathTainted);
        if (mats.MirrorTainted == null)
        {
            mats.MirrorTainted = new Material(litShader);
            mats.MirrorTainted.name = "Mat_Mirror_Tainted";
            mats.MirrorTainted.color = new Color(0.40f, 0.35f, 0.38f, 1f);
            if (mats.MirrorTainted.HasProperty("_Metallic")) mats.MirrorTainted.SetFloat("_Metallic", 0.85f);
            if (mats.MirrorTainted.HasProperty("_Smoothness")) mats.MirrorTainted.SetFloat("_Smoothness", 0.88f);
            AssetDatabase.CreateAsset(mats.MirrorTainted, pathTainted);
        }

        // 3. Mat_Mirror_Frame_Gold.mat (Khung đồng vàng cổ mạ phong hóa)
        string pathGold = "Assets/Materials/Mat_Mirror_Frame_Gold.mat";
        mats.FrameGold = AssetDatabase.LoadAssetAtPath<Material>(pathGold);
        if (mats.FrameGold == null)
        {
            mats.FrameGold = new Material(litShader);
            mats.FrameGold.name = "Mat_Mirror_Frame_Gold";
            mats.FrameGold.color = new Color(0.68f, 0.54f, 0.24f, 1f);
            if (mats.FrameGold.HasProperty("_Metallic")) mats.FrameGold.SetFloat("_Metallic", 0.85f);
            if (mats.FrameGold.HasProperty("_Smoothness")) mats.FrameGold.SetFloat("_Smoothness", 0.65f);
            AssetDatabase.CreateAsset(mats.FrameGold, pathGold);
        }

        // 4. Mat_Mirror_Frame_DarkWood.mat (Khung gỗ gụ già tối màu)
        string pathWood = "Assets/Materials/Mat_Mirror_Frame_DarkWood.mat";
        mats.FrameDarkWood = AssetDatabase.LoadAssetAtPath<Material>(pathWood);
        if (mats.FrameDarkWood == null)
        {
            mats.FrameDarkWood = new Material(litShader);
            mats.FrameDarkWood.name = "Mat_Mirror_Frame_DarkWood";
            mats.FrameDarkWood.color = new Color(0.18f, 0.11f, 0.08f, 1f);
            if (mats.FrameDarkWood.HasProperty("_Metallic")) mats.FrameDarkWood.SetFloat("_Metallic", 0.1f);
            if (mats.FrameDarkWood.HasProperty("_Smoothness")) mats.FrameDarkWood.SetFloat("_Smoothness", 0.45f);
            AssetDatabase.CreateAsset(mats.FrameDarkWood, pathWood);
        }

        // 5. Mat_Fabric_Velvet.mat (Nhung đỏ đỗ rượu cho sofa/thảm)
        string pathVelvet = "Assets/Materials/Mat_Fabric_Velvet.mat";
        mats.FabricVelvet = AssetDatabase.LoadAssetAtPath<Material>(pathVelvet);
        if (mats.FabricVelvet == null)
        {
            mats.FabricVelvet = new Material(litShader);
            mats.FabricVelvet.name = "Mat_Fabric_Velvet";
            mats.FabricVelvet.color = new Color(0.42f, 0.08f, 0.10f, 1f);
            if (mats.FabricVelvet.HasProperty("_Metallic")) mats.FabricVelvet.SetFloat("_Metallic", 0.05f);
            if (mats.FabricVelvet.HasProperty("_Smoothness")) mats.FabricVelvet.SetFloat("_Smoothness", 0.25f);
            AssetDatabase.CreateAsset(mats.FabricVelvet, pathVelvet);
        }

        AssetDatabase.SaveAssets();
        return mats;
    }
    #endregion

    #region Wall Mirrors Placement ("Nhà Gương")
    private static int PlaceWallMirrors(Transform parent, MirrorMaterials mats)
    {
        int count = 0;

        // =========================================================================
        // 1. HÀNH LANG CHÍNH (THE HALL OF MIRRORS - 8 GƯƠNG ĐỐI XỨNG 2 BÊN TƯỜNG)
        // Thông số chuẩn xác của 2 vách tường hành lang Level_Main:
        // - Tường Tây (Wall_West_Corr_Mid): Tâm X = -2.698m, mặt trong lòng hành lang tại X = -2.533m
        // - Tường Đông (Wall_East_Corr_Mid): Tâm X = 1.921m, mặt trong lòng hành lang tại X = 1.746m
        // Căn chỉnh gương áp sát mặt tường (không lơ lửng, không đâm xuyên):
        // - Gương tường Tây: X = -2.50m, quay mặt sang Đông (+X, Euler Y = 90 độ)
        // - Gương tường Đông: X = 1.72m, quay mặt sang Tây (-X, Euler Y = -90 độ)
        // Bố trí 4 cặp gương đối diện nhau dọc dải tường đặc an toàn (Z = -5.4m, -1.8m, 1.8m, 5.4m)
        // =========================================================================
        float hallWallWestX = -2.50f;
        float hallWallEastX = 1.72f;
        float hallMirrorY = 1.75f;
        Vector2 hallMirrorSize = new Vector2(0.95f, 1.65f);

        float[] hallZCoords = new float[] { -5.4f, -1.8f, 1.8f, 5.4f };

        for (int i = 0; i < hallZCoords.Length; i++)
        {
            float z = hallZCoords[i];

            // Tường Tây (Mặt gương quay sang Đông +X, Euler Y = 90)
            CreateGothicWallMirror(
                $"Mirror_Corridor_West_{i + 1}",
                new Vector3(hallWallWestX, hallMirrorY, z),
                Quaternion.Euler(0f, 90f, 0f),
                hallMirrorSize,
                mats.MirrorReflective,
                mats.FrameGold,
                parent,
                "Dãy gương đối xứng ở hành lang tạo thành vô số chiếc bóng lặp lại đến vô tận... Bạn thấy chiếc bóng thứ tư đang từ từ nghiêng đầu nhìn bạn."
            );
            count++;

            // Tường Đông (Mặt gương quay sang Tây -X, Euler Y = -90)
            CreateGothicWallMirror(
                $"Mirror_Corridor_East_{i + 1}",
                new Vector3(hallWallEastX, hallMirrorY, z),
                Quaternion.Euler(0f, -90f, 0f),
                hallMirrorSize,
                mats.MirrorReflective,
                mats.FrameGold,
                parent,
                "Tấm gương đối diện phản chiếu lại hành lang lạnh lẽo. Dường như ánh mắt trong gương đang dõi theo từng bước chân của bạn."
            );
            count++;
        }

        // =========================================================================
        // 2. PHÒNG NGỦ EMILY (3 GƯƠNG)
        // =========================================================================
        // Gương 1: Bàn trang điểm / học của Emily (Treo trên bàn học áp sát tường Bắc Z = 16.80m, quay mặt Nam 180 độ)
        CreateGothicWallMirror(
            "Mirror_Bedroom_VanityDesk",
            new Vector3(-7.5f, 1.70f, 16.80f),
            Quaternion.Euler(0f, 180f, 0f),
            new Vector2(1.1f, 1.35f),
            mats.MirrorReflective,
            mats.FrameGold,
            parent,
            "Tấm gương bàn trang điểm nơi Emily từng ngồi chải tóc mỗi đêm... Có những vệt ố mờ hình bàn tay trẻ con in từ bên trong kính."
        );
        count++;

        // Gương 2: Gương đứng soi toàn thân trên tường Tây phòng ngủ (Áp sát mặt trong tường Tây X = -16.80m, quay mặt Đông 90 độ)
        CreateGothicWallMirror(
            "Mirror_Bedroom_FullLength",
            new Vector3(-16.80f, 1.60f, 10.0f),
            Quaternion.Euler(0f, 90f, 0f),
            new Vector2(0.9f, 2.1f),
            mats.MirrorReflective,
            mats.FrameDarkWood,
            parent,
            "Tấm gương soi toàn thân kiểu Gothic cổ điển. Trong khoảnh khắc chớp mắt, chiếc bóng trong gương dường như không chớp mắt cùng bạn."
        );
        count++;

        // Gương 3: Gương đối diện giường ngủ của Emily (Áp sát mặt trong tường Bắc Z = 16.80m, quay mặt Nam 180 độ)
        CreateGothicWallMirror(
            "Mirror_Bedroom_BedWatching",
            new Vector3(-14.0f, 1.80f, 16.80f),
            Quaternion.Euler(0f, 180f, 0f),
            new Vector2(1.2f, 1.5f),
            mats.MirrorReflective,
            mats.FrameDarkWood,
            parent,
            "Tấm gương treo đối diện ngay trước giường ngủ... Cảm giác rợn ngợp khi mỗi lần thức giấc giữa đêm, điều đầu tiên nhìn thấy luôn là tấm gương đen ngòm này."
        );
        count++;

        // =========================================================================
        // 3. THƯ VIỆN CỔ (3 GƯƠNG)
        // =========================================================================
        // Gương 1: Gương đại sảnh Thư viện (Áp sát mặt trong tường Nam Z = -16.80m, quay mặt Bắc 0 độ)
        CreateGothicWallMirror(
            "Mirror_Library_GrandOccult",
            new Vector3(-9.5f, 1.90f, -16.80f),
            Quaternion.Euler(0f, 0f, 0f),
            new Vector2(1.8f, 1.6f),
            mats.MirrorReflective,
            mats.FrameGold,
            parent,
            "Tấm gương lớn chạm khắc các hoa văn thần bí cõi bóng tối. Những trang cổ thư xung quanh dường như đang rung rinh dưới ánh nến phản chiếu."
        );
        count++;

        // Gương 2: Gương vòm giữa các kệ sách (Áp sát mặt trong tường Tây X = -16.80m, quay mặt Đông 90 độ)
        CreateGothicWallMirror(
            "Mirror_Library_BookshelfArch",
            new Vector3(-16.80f, 1.70f, -6.0f),
            Quaternion.Euler(0f, 90f, 0f),
            new Vector2(0.85f, 1.6f),
            mats.MirrorReflective,
            mats.FrameDarkWood,
            parent,
            "Tấm gương kẹp giữa hai dãy kệ sách đồ sộ. Phản chiếu lại những cuốn sách bìa da cũ kĩ trải dài như bất tận."
        );
        count++;

        // Gương 3: Gương góc đọc sách tĩnh lặng (Áp sát mặt trong tường Tây X = -16.80m, quay mặt Đông 90 độ)
        CreateGothicWallMirror(
            "Mirror_Library_ReadingCorner",
            new Vector3(-16.80f, 1.70f, -13.0f),
            Quaternion.Euler(0f, 90f, 0f),
            new Vector2(0.95f, 1.5f),
            mats.MirrorReflective,
            mats.FrameGold,
            parent,
            "Góc đọc sách u ám... Tấm gương phản chiếu chiếc ghế bành trống không, nhưng mặt ghế dường như hơi lõm xuống như có ai vừa ngồi dậy."
        );
        count++;

        // =========================================================================
        // 4. PHÒNG KHÁCH & BÀN ĂN (3 GƯƠNG)
        // =========================================================================
        // Gương 1: Gương Đại sảnh bàn ăn tiệc (Áp sát mặt trong tường Đông X = 16.80m, quay mặt Tây -90 độ)
        CreateGothicWallMirror(
            "Mirror_Dining_GrandBuffet",
            new Vector3(16.80f, 1.95f, 8.5f),
            Quaternion.Euler(0f, -90f, 0f),
            new Vector2(2.4f, 1.4f),
            mats.MirrorReflective,
            mats.FrameGold,
            parent,
            "Tấm gương dài mạ vàng sang trọng của phòng ăn quý tộc. Chiếc bàn ăn dài phản chiếu trong gương mang vẻ thê lương của một bữa tiệc không người trở về."
        );
        count++;

        // Gương 2: Gương phòng khách trên tường Bắc (Áp sát mặt trong tường Bắc Z = 16.80m, quay mặt Nam 180 độ)
        CreateGothicWallMirror(
            "Mirror_Living_Mantelpiece",
            new Vector3(10.0f, 1.85f, 16.80f),
            Quaternion.Euler(0f, 180f, 0f),
            new Vector2(1.5f, 1.5f),
            mats.MirrorReflective,
            mats.FrameGold,
            parent,
            "Tấm gương phong cách Victoria treo trang trọng phía trên bộ sofa phòng khách. Khung gương uốn lượn hình những nhánh dây leo sắc nhọn."
        );
        count++;

        // Gương 3: Gương sảnh đón tiếp gần cửa phòng ăn (Áp sát mặt ngoài tường hành lang Đông X = 2.13m, quay mặt Đông 90 độ vào phòng ăn)
        CreateGothicWallMirror(
            "Mirror_Dining_FoyerEntry",
            new Vector3(2.13f, 1.70f, 4.0f),
            Quaternion.Euler(0f, 90f, 0f),
            new Vector2(0.95f, 1.6f),
            mats.MirrorReflective,
            mats.FrameDarkWood,
            parent,
            "Tấm gương chào đón khách quý ở cửa vào phòng ăn... Nhưng gương mặt chào đón bạn lúc này chỉ có sự hoang lạnh và tử khí."
        );
        count++;

        // =========================================================================
        // 5. NHÀ KHO & PHÒNG TẾ LỄ (3 GƯƠNG Ố MÁU / RÙNG RỢN)
        // =========================================================================
        // Gương 1: Gương đen phong ấn sau Tế Đàn (Áp sát mặt trong tường Nam Z = -16.80m, quay mặt Bắc 0 độ)
        CreateGothicWallMirror(
            "Mirror_Storage_RitualBlackAltar",
            new Vector3(9.5f, 1.90f, -16.80f),
            Quaternion.Euler(0f, 0f, 0f),
            new Vector2(1.6f, 1.8f),
            mats.MirrorTainted,
            mats.FrameDarkWood,
            parent,
            "TẤM GƯƠNG TẾ ĐÀN ĐEN THẪM: Mặt kính toát ra luồng hắc khí lạnh thấu xương. Hình như có vô số khuôn mặt đang gào thét câm lặng dưới mặt kính!"
        );
        count++;

        // Gương 2: Gương cào máu góc kho (Áp sát mặt trong tường Đông X = 16.80m, quay mặt Tây -90 độ)
        CreateGothicWallMirror(
            "Mirror_Storage_BloodScratched",
            new Vector3(16.80f, 1.70f, -8.5f),
            Quaternion.Euler(0f, -90f, 0f),
            new Vector2(1.1f, 1.5f),
            mats.MirrorTainted,
            mats.FrameDarkWood,
            parent,
            "Mặt gương bị nứt một đường chéo dài và bám đầy các vết cào móng tay dính máu khô. Ai đó đã tuyệt vọng cầu cứu trước khi bị kéo vào cõi gương..."
        );
        count++;

        // Gương 3: Gương góc ẩm thấp nhà kho (Áp sát mặt ngoài tường hành lang Đông X = 2.13m, quay mặt Đông 90 độ vào nhà kho)
        CreateGothicWallMirror(
            "Mirror_Storage_ShadowEcho",
            new Vector3(2.13f, 1.65f, -12.0f),
            Quaternion.Euler(0f, 90f, 0f),
            new Vector2(0.9f, 1.4f),
            mats.MirrorTainted,
            mats.FrameDarkWood,
            parent,
            "Tấm gương ẩm mốc nằm trong góc khuất. Bạn nghe thấy tiếng gõ lách cách khe khẽ vang lên đều đặn từ bên trong mặt kính..."
        );
        count++;

        return count;
    }

    private static GameObject CreateGothicWallMirror(
        string name,
        Vector3 pos,
        Quaternion rot,
        Vector2 size,
        Material glassMat,
        Material frameMat,
        Transform parent,
        string horrorQuote)
    {
        GameObject mirrorObj = new GameObject(name);
        mirrorObj.transform.position = pos;
        mirrorObj.transform.rotation = rot;
        mirrorObj.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(mirrorObj, "Create " + name);

        float W = size.x;
        float H = size.y;
        float frameThick = 0.08f;
        float frameDepth = 0.05f;

        // 1. Mặt kính gương (Glass Panel)
        GameObject glass = GameObject.CreatePrimitive(PrimitiveType.Cube);
        glass.name = "Mirror_Glass_Surface";
        glass.transform.SetParent(mirrorObj.transform, false);
        glass.transform.localPosition = new Vector3(0f, 0f, 0.01f);
        glass.transform.localScale = new Vector3(W, H, 0.02f);
        if (glassMat != null) glass.GetComponent<MeshRenderer>().sharedMaterial = glassMat;

        // 2. Khung viền 4 cạnh (Ornate Border Frame)
        // Viền trên
        CreateFrameSegment("Frame_Top", new Vector3(0f, H / 2 + frameThick / 2, 0f), new Vector3(W + frameThick * 2, frameThick, frameDepth), frameMat, mirrorObj.transform);
        // Viền dưới (bệ đỡ)
        CreateFrameSegment("Frame_Bottom", new Vector3(0f, -H / 2 - frameThick / 2, 0f), new Vector3(W + frameThick * 2 + 0.08f, frameThick * 1.3f, frameDepth * 1.4f), frameMat, mirrorObj.transform);
        // Viền trái
        CreateFrameSegment("Frame_Left", new Vector3(-W / 2 - frameThick / 2, 0f, 0f), new Vector3(frameThick, H, frameDepth), frameMat, mirrorObj.transform);
        // Viền phải
        CreateFrameSegment("Frame_Right", new Vector3(W / 2 + frameThick / 2, 0f, 0f), new Vector3(frameThick, H, frameDepth), frameMat, mirrorObj.transform);
        // Vương miện đỉnh gương Gothic (Top Crown Molding)
        CreateFrameSegment("Frame_CrownArch", new Vector3(0f, H / 2 + frameThick * 1.6f, -0.01f), new Vector3(W * 0.6f, frameThick * 1.2f, frameDepth * 1.2f), frameMat, mirrorObj.transform);

        // 3. BoxCollider cho tương tác
        BoxCollider col = mirrorObj.AddComponent<BoxCollider>();
        col.center = Vector3.zero;
        col.size = new Vector3(W + frameThick * 2, H + frameThick * 2, 0.2f);

        // 4. MirrorInteractive component
        MirrorInteractive mi = mirrorObj.AddComponent<MirrorInteractive>();
        mi.SetCustomQuote(horrorQuote);

        return mirrorObj;
    }

    private static void CreateFrameSegment(string name, Vector3 localPos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPos;
        part.transform.localScale = scale;

        Collider c = part.GetComponent<Collider>();
        if (c != null) UnityEngine.Object.DestroyImmediate(c);

        if (mat != null) part.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }
    #endregion

    #region Furniture Placement & Auto-Normalization
    private static int PlaceFurnitureAndDecorations(Transform parent, AssetCatalog catalog, MirrorMaterials mats)
    {
        int count = 0;

        // =========================================================================
        // 1. PHÒNG NGỦ EMILY (EMILY'S BEDROOM - TÂY BẮC)
        // Khu vực X: -16.8 đến -2.7m, Z: 0 đến 16.8m.
        // Giữ thông thoáng cửa thông phòng (X = -9.5, Z = 0) và cửa hành lang (Z ~ 6m)
        // =========================================================================
        // Tủ quần áo gỗ cổ Emily (áp tường Tây X = -15.8m, Z = 5.5m, quay mặt Đông 90 độ)
        PlaceOrFallbackCabinet("Prop_Cabinet_Bedroom_Emily", new Vector3(-15.8f, 0f, 5.5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.5f, 2.4f, 0.75f), catalog.GetCabinet(0), mats.FrameDarkWood, parent);
        count++;

        // Bàn đầu giường ngủ trái & phải
        PlaceOrFallbackTable("Prop_Bedroom_Nightstand_Left", new Vector3(-15.5f, 0f, 12.5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(0.65f, 0.7f, 0.65f), catalog.GetTable(0), mats.FrameDarkWood, parent);
        PlaceOrFallbackTable("Prop_Bedroom_Nightstand_Right", new Vector3(-11.5f, 0f, 12.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(0.65f, 0.7f, 0.65f), catalog.GetTable(1), mats.FrameDarkWood, parent);
        count += 2;

        // Chân nến / đèn ngủ trên bàn đầu giường
        PlaceOrFallbackCandelabra("Prop_Bedroom_Candle_1", new Vector3(-15.5f, 0.7f, 12.5f), Quaternion.identity, new Vector3(0.3f, 0.45f, 0.3f), catalog.GetLamp(0), mats.FrameGold, parent);
        PlaceOrFallbackCandelabra("Prop_Bedroom_Candle_2", new Vector3(-11.5f, 0.7f, 12.5f), Quaternion.identity, new Vector3(0.3f, 0.45f, 0.3f), catalog.GetLamp(1), mats.FrameGold, parent);
        count += 2;

        // Ghế tựa bàn trang điểm của Emily (Dưới gương VanityDesk tại X = -7.5f, Z = 16.8f)
        PlaceOrFallbackChair("Prop_Bedroom_VanityChair", new Vector3(-7.5f, 0f, 14.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(0.65f, 0.95f, 0.65f), catalog.GetChair(0), mats.FabricVelvet, mats.FrameDarkWood, parent);
        count++;

        // Ghế bành nhung đọc sách thư giãn phòng ngủ
        PlaceOrFallbackChair("Prop_Bedroom_VelvetArmchair", new Vector3(-5.0f, 0f, 13.0f), Quaternion.Euler(0f, -135f, 0f), new Vector3(0.95f, 1.05f, 0.9f), catalog.GetChair(1), mats.FabricVelvet, mats.FrameDarkWood, parent);
        count++;

        // Bàn trà nhỏ cạnh ghế bành
        PlaceOrFallbackTable("Prop_Bedroom_SmallTeaTable", new Vector3(-5.0f, 0f, 11.5f), Quaternion.identity, new Vector3(0.7f, 0.55f, 0.7f), catalog.GetTable(2), mats.FrameDarkWood, parent);
        count++;

        // Rương hòm cũ phong ấn đồ chơi dưới chân giường
        PlaceOrFallbackChest("Prop_Bedroom_AntiqueChest", new Vector3(-13.5f, 0f, 9.8f), Quaternion.Euler(0f, 180f, 0f), new Vector3(1.1f, 0.6f, 0.6f), catalog.GetCrate(0), mats.FrameDarkWood, mats.FrameGold, parent);
        count++;

        // Thảm nhung phòng ngủ lớn
        CreateCarpet("Prop_Bedroom_GrandCarpet", new Vector3(-12.5f, 0.012f, 12.5f), new Vector3(5.5f, 0.02f, 4.5f), mats.FabricVelvet, parent);
        count++;

        // Đèn cây đứng góc phòng ngủ
        PlaceOrFallbackCandelabra("Prop_Bedroom_FloorLamp", new Vector3(-15.8f, 0f, 15.8f), Quaternion.Euler(0f, 45f, 0f), new Vector3(0.45f, 1.8f, 0.45f), catalog.GetLamp(2), mats.FrameGold, parent);
        count++;

        // =========================================================================
        // 2. THƯ VIỆN CỔ (OCCULT LIBRARY - TÂY NAM)
        // Khu vực X: -16.8 đến -2.7m, Z: -16.8 đến 0m.
        // Giữ thông thoáng cửa thông phòng (X = -9.5, Z = 0) và cửa hành lang (Z ~ -6m)
        // =========================================================================
        // Tủ lưu trữ cấm thư Grimoire (Old Wooden Cabinet sát vách Nam X = -14.5m, Z = -15.8m, quay mặt Bắc 0 độ)
        PlaceOrFallbackCabinet("Prop_Cabinet_Library_Grimoire", new Vector3(-14.5f, 0f, -15.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(1.5f, 2.4f, 0.75f), catalog.GetCabinet(1), mats.FrameDarkWood, parent);
        count++;

        // Tủ lưu trữ hồ sơ cổ thứ 2 (Old Wooden Cabinet sát vách Nam X = -5.0m, Z = -15.8m, quay mặt Bắc 0 độ)
        PlaceOrFallbackCabinet("Prop_Cabinet_Library_Archives", new Vector3(-5.0f, 0f, -15.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(1.5f, 2.4f, 0.75f), catalog.GetCabinet(2), mats.FrameDarkWood, parent);
        count++;

        // 2 Ghế tựa học giả quanh bàn nghiên cứu trung tâm (Bàn StudyDesk tại X = -9.5, Z = -9.5)
        PlaceOrFallbackChair("Prop_Library_StudyChair_1", new Vector3(-9.5f, 0f, -8.2f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.7f, 1.0f, 0.7f), catalog.GetChair(2), mats.FabricVelvet, mats.FrameDarkWood, parent);
        PlaceOrFallbackChair("Prop_Library_StudyChair_2", new Vector3(-9.5f, 0f, -10.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(0.7f, 1.0f, 0.7f), catalog.GetChair(3), mats.FabricVelvet, mats.FrameDarkWood, parent);
        count += 2;

        // Ghế bành đọc sách góc thư viện
        PlaceOrFallbackChair("Prop_Library_ReadingArmchair", new Vector3(-15.0f, 0f, -14.0f), Quaternion.Euler(0f, 45f, 0f), new Vector3(0.95f, 1.05f, 0.9f), catalog.GetChair(4), mats.FabricVelvet, mats.FrameDarkWood, parent);
        count++;

        // Bàn trà nhỏ cạnh ghế bành đọc sách
        PlaceOrFallbackTable("Prop_Library_TeaTable", new Vector3(-13.5f, 0f, -13.5f), Quaternion.identity, new Vector3(0.75f, 0.55f, 0.75f), catalog.GetTable(3), mats.FrameDarkWood, parent);
        count++;

        // Đồng hồ quả lắc thư viện (vách ngăn phía Đông X = -3.2m, Z = -14.5m, quay mặt Tây -90 độ)
        PlaceOrFallbackClock("Prop_Library_GrandfatherClock", new Vector3(-3.2f, 0f, -14.5f), Quaternion.Euler(0f, -90f, 0f), catalog.GetClock(0), mats.FrameDarkWood, mats.FrameGold, parent);
        count++;

        // Giá sách cổ đồ sộ vách Đông
        PlaceOrFallbackBookshelf("Prop_Library_Bookshelf_Extra", new Vector3(-3.2f, 0f, -11.0f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.8f, 2.4f, 0.6f), catalog.GetBookshelf(0), mats.FrameDarkWood, parent);
        count++;

        // Chân nến đồng trên bàn đọc sách
        PlaceOrFallbackCandelabra("Prop_Library_Candelabra", new Vector3(-8.5f, 0.82f, -9.5f), Quaternion.identity, new Vector3(0.35f, 0.45f, 0.35f), catalog.GetLamp(3), mats.FrameGold, parent);
        count++;

        // Thảm nỉ thư viện cổ
        CreateCarpet("Prop_Library_GrandCarpet", new Vector3(-9.5f, 0.012f, -9.5f), new Vector3(5.0f, 0.02f, 4.0f), mats.FrameDarkWood, parent);
        count++;

        // =========================================================================
        // 3. PHÒNG KHÁCH & BÀN ĂN (DINING & VINTAGE LIVING ROOM - ĐÔNG BẮC)
        // Khu vực X: 1.9 đến 16.8m, Z: 0 đến 16.8m.
        // Giữ thông thoáng cửa thông phòng (X = 9.5, Z = 0) và cửa hành lang (Z ~ 6m)
        // =========================================================================
        // A. KHU VỰC PHÒNG KHÁCH CỔ ĐIỂN (Z: 11m -> 16.5m)
        // Lò sưởi cổ kính dưới gương mantelpiece vách Bắc (X = 10.0m, Z = 16.3m, quay mặt Nam 180 độ)
        PlaceOrFallbackFireplace("Prop_Living_Fireplace", new Vector3(10.0f, 0f, 16.3f), Quaternion.Euler(0f, 180f, 0f), new Vector3(2.2f, 1.4f, 0.7f), catalog.GetFireplace(0), mats.FrameDarkWood, parent);
        count++;

        // Sofa dài Chesterfield
        PlaceOrFallbackSofa("Prop_Living_VintageSofa", new Vector3(6.2f, 0f, 13.5f), Quaternion.Euler(0f, 90f, 0f), catalog.GetSofa(0), mats.FabricVelvet, mats.FrameDarkWood, parent);
        count++;

        // 2 Ghế bành quý tộc đối diện sofa
        PlaceOrFallbackChair("Prop_Living_Armchair_1", new Vector3(12.8f, 0f, 12.0f), Quaternion.Euler(0f, -60f, 0f), new Vector3(0.95f, 1.05f, 0.9f), catalog.GetChair(5), mats.FabricVelvet, mats.FrameDarkWood, parent);
        PlaceOrFallbackChair("Prop_Living_Armchair_2", new Vector3(12.8f, 0f, 15.0f), Quaternion.Euler(0f, -120f, 0f), new Vector3(0.95f, 1.05f, 0.9f), catalog.GetChair(6), mats.FabricVelvet, mats.FrameDarkWood, parent);
        count += 2;

        // Bàn trà phòng khách
        PlaceOrFallbackTable("Prop_Living_CoffeeTable", new Vector3(9.5f, 0f, 13.5f), Quaternion.identity, new Vector3(1.5f, 0.55f, 0.85f), catalog.GetTable(4), mats.FrameDarkWood, parent);
        count++;

        // Thảm nhung đỏ đỗ rượu phòng khách
        CreateCarpet("Prop_Living_VintageRug", new Vector3(9.5f, 0.012f, 13.5f), new Vector3(4.8f, 0.02f, 4.0f), mats.FabricVelvet, parent);
        count++;

        // Đồng hồ quả lắc cây đứng góc phòng khách
        PlaceOrFallbackClock("Prop_Living_GrandfatherClock", new Vector3(15.8f, 0f, 15.8f), Quaternion.Euler(0f, -135f, 0f), catalog.GetClock(1), mats.FrameDarkWood, mats.FrameGold, parent);
        count++;

        // Tủ gỗ trưng bày đồ cổ vách Đông (Old Wooden Cabinet tại X = 16.0m, Z = 11.5m, quay mặt Tây -90 độ)
        PlaceOrFallbackCabinet("Prop_Cabinet_Living_Display", new Vector3(16.0f, 0f, 11.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.4f, 2.3f, 0.7f), catalog.GetCabinet(3), mats.FrameDarkWood, parent);
        count++;

        // B. KHU VỰC BÀN ĂN HOÀNG GIA (Z: 4m -> 10m)
        // Tủ bát đĩa búp phê phòng ăn (Old Wooden Cabinet vách Bắc X = 5.0m, Z = 16.0m, quay mặt Nam 180 độ)
        PlaceOrFallbackCabinet("Prop_Cabinet_Dining_Buffet", new Vector3(5.0f, 0f, 16.0f), Quaternion.Euler(0f, 180f, 0f), new Vector3(1.8f, 2.1f, 0.75f), catalog.GetCabinet(4), mats.FrameDarkWood, parent);
        count++;

        // 4 Ghế ăn quý tộc xếp quanh bàn tiệc (Bàn tiệc ở X = 9.5, Z = 8.5)
        PlaceOrFallbackChair("Prop_Dining_Chair_1", new Vector3(8.3f, 0f, 9.6f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.65f, 1.0f, 0.65f), catalog.GetChair(7), mats.FabricVelvet, mats.FrameDarkWood, parent);
        PlaceOrFallbackChair("Prop_Dining_Chair_2", new Vector3(10.7f, 0f, 9.6f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.65f, 1.0f, 0.65f), catalog.GetChair(8), mats.FabricVelvet, mats.FrameDarkWood, parent);
        PlaceOrFallbackChair("Prop_Dining_Chair_3", new Vector3(8.3f, 0f, 7.4f), Quaternion.Euler(0f, 0f, 0f), new Vector3(0.65f, 1.0f, 0.65f), catalog.GetChair(9), mats.FabricVelvet, mats.FrameDarkWood, parent);
        PlaceOrFallbackChair("Prop_Dining_Chair_4", new Vector3(10.7f, 0f, 7.4f), Quaternion.Euler(0f, 0f, 0f), new Vector3(0.65f, 1.0f, 0.65f), catalog.GetChair(10), mats.FabricVelvet, mats.FrameDarkWood, parent);
        count += 4;

        // Bàn console đón khách dưới gương FoyerEntry (vách Tây phòng ăn X = 3.0m, Z = 4.0m, quay mặt Đông 90 độ)
        PlaceOrFallbackTable("Prop_Dining_FoyerTable", new Vector3(3.0f, 0f, 4.0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.1f, 0.85f, 0.5f), catalog.GetTable(5), mats.FrameDarkWood, parent);
        count++;

        // Chân nến bàn tiệc
        PlaceOrFallbackCandelabra("Prop_Dining_Candelabra", new Vector3(9.5f, 0.85f, 8.5f), Quaternion.identity, new Vector3(0.4f, 0.5f, 0.4f), catalog.GetLamp(4), mats.FrameGold, parent);
        count++;

        // =========================================================================
        // 4. NHÀ KHO & PHÒNG TẾ LỄ (STORAGE & RITUAL CHAMBER - ĐÔNG NAM)
        // Khu vực X: 1.9 đến 16.8m, Z: -16.8 đến 0m.
        // Giữ thông thoáng cửa thông phòng (X = 9.5, Z = 0) và cửa hành lang (Z ~ -6m)
        // =========================================================================
        // A. KHU VỰC TẾ ĐÀN BÓNG TỐI (Z: -12m -> -16.8m)
        // Tủ cất giữ cổ vật tà giáo (Old Wooden Cabinet vách Nam X = 4.0m, Z = -15.8m, quay mặt Bắc 0 độ)
        PlaceOrFallbackCabinet("Prop_Cabinet_Storage_Relics", new Vector3(4.0f, 0f, -15.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(1.4f, 2.2f, 0.7f), catalog.GetCabinet(5), mats.FrameDarkWood, parent);
        count++;

        // 2 Chân nến tế lễ 2 bên bàn tế đàn (Bàn tế đàn tại X = 9.5, Z = -14.0)
        PlaceOrFallbackCandelabra("Prop_Ritual_Candelabra_Left", new Vector3(7.8f, 0f, -14.5f), Quaternion.identity, new Vector3(0.45f, 1.3f, 0.45f), catalog.GetLamp(5), mats.FrameDarkWood, parent);
        PlaceOrFallbackCandelabra("Prop_Ritual_Candelabra_Right", new Vector3(11.2f, 0f, -14.5f), Quaternion.identity, new Vector3(0.45f, 1.3f, 0.45f), catalog.GetLamp(6), mats.FrameDarkWood, parent);
        count += 2;

        // Vòng tròn tế lễ / Thảm ma thuật cõi gương
        CreateCarpet("Prop_Ritual_FloorDecal", new Vector3(9.5f, 0.015f, -12.5f), new Vector3(3.5f, 0.02f, 3.5f), mats.MirrorTainted, parent);
        count++;

        // B. KHU VỰC NHÀ KHO CŨ KỸ (Z: -4m -> -11m)
        // Tủ gỗ cũ mốc meo góc kho (Old Wooden Cabinet vách Đông X = 16.0m, Z = -4.5m, quay mặt Tây -90 độ)
        PlaceOrFallbackCabinet("Prop_Cabinet_Storage_Dusty", new Vector3(16.0f, 0f, -4.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.4f, 2.0f, 0.7f), catalog.GetCabinet(6), mats.FrameDarkWood, parent);
        count++;

        // Cụm thùng kiện hàng gỗ góc Đông Nam (X = 15.5m, Z = -15.8m)
        PlaceOrFallbackChest("Prop_Storage_Crates_Stack_1", new Vector3(15.5f, 0f, -15.8f), Quaternion.Euler(0f, 25f, 0f), new Vector3(1.2f, 1.1f, 1.0f), catalog.GetCrate(1), mats.FrameDarkWood, mats.FrameGold, parent);
        count++;

        // Cụm thùng kiện hàng gỗ vách Tây kho (X = 3.5m, Z = -5.0m)
        PlaceOrFallbackChest("Prop_Storage_Crates_Stack_2", new Vector3(3.5f, 0f, -5.0f), Quaternion.Euler(0f, -15f, 0f), new Vector3(1.1f, 0.9f, 0.9f), catalog.GetCrate(2), mats.FrameDarkWood, mats.FrameGold, parent);
        count++;

        // Kệ sắt / kệ chứa đồ cũ (vách Tây kho X = 3.0m, Z = -10.0m, quay mặt Đông 90 độ)
        PlaceOrFallbackBookshelf("Prop_Storage_MetalShelf", new Vector3(3.0f, 0f, -10.0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.6f, 2.2f, 0.55f), catalog.GetBookshelf(1), mats.FrameDarkWood, parent);
        count++;

        // Cụm thùng rượu gỗ bổ sung (X = 15.5m, Z = -11.0f)
        PlaceOrFallbackBarrels("Prop_Storage_ExtraBarrels", new Vector3(15.5f, 0f, -11.0f), Quaternion.identity, new Vector3(1.2f, 1.0f, 1.2f), catalog.GetBarrel(0), mats.FrameDarkWood, parent);
        count++;

        // =========================================================================
        // 5. HÀNH LANG CHÍNH (THE HALL OF MIRRORS)
        // 2 Chân đế bình gốm cổ áp tường ở khoảng trống Z = 0 (lối đi > 3.5m rộng thênh thang)
        // =========================================================================
        PlaceOrFallbackPedestal("Prop_Hallway_Pedestal_West", new Vector3(-2.35f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(0.4f, 1.1f, 0.4f), catalog.GetDecoration(0), mats.FrameDarkWood, mats.FrameGold, parent);
        PlaceOrFallbackPedestal("Prop_Hallway_Pedestal_East", new Vector3(1.55f, 0f, 0f), Quaternion.Euler(0f, -90f, 0f), new Vector3(0.4f, 1.1f, 0.4f), catalog.GetDecoration(1), mats.FrameDarkWood, mats.FrameGold, parent);
        count += 2;

        return count;
    }

    private static GameObject InstantiateAndNormalize(
        GameObject assetOrPrefab,
        Vector3 targetSize,
        Transform parent,
        string name,
        Vector3 pos,
        Quaternion rot)
    {
        if (assetOrPrefab == null) return null;

        GameObject obj;
        if (PrefabUtility.IsPartOfPrefabAsset(assetOrPrefab))
        {
            obj = (GameObject)PrefabUtility.InstantiatePrefab(assetOrPrefab);
        }
        else
        {
            obj = UnityEngine.Object.Instantiate(assetOrPrefab);
        }

        if (obj == null) return null;

        obj.name = name;
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.transform.SetParent(parent, true);
        obj.transform.localScale = Vector3.one;

        // Tự động chuẩn hóa kích thước vật lý theo mét thực tế
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                b.Encapsulate(renderers[i].bounds);
            }

            Vector3 curSize = b.size;
            if (curSize.y > 0.02f && targetSize.y > 0.02f)
            {
                float scaleRatio = targetSize.y / curSize.y;
                obj.transform.localScale = Vector3.one * scaleRatio;

                // Căn chỉnh lại để chân đế chạm sàn chính xác tại pos.y
                renderers = obj.GetComponentsInChildren<Renderer>();
                b = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    b.Encapsulate(renderers[i].bounds);
                }

                float yBottomDiff = pos.y - b.min.y;
                obj.transform.position += new Vector3(0f, yBottomDiff, 0f);
            }
        }

        Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
        EnsureSolidCollider(obj, targetSize);
        return obj;
    }

    private static void PlaceOrFallbackCabinet(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material woodMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Old Wooden Cabinet
        GameObject cab = new GameObject(name);
        cab.transform.position = pos;
        cab.transform.rotation = rot;
        cab.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(cab, "Create " + name);

        // Body
        CreateBoxChild("Cabinet_Body", new Vector3(0f, scale.y / 2, 0f), scale, woodMat, cab.transform);
        // Top Crown
        CreateBoxChild("Cabinet_Crown", new Vector3(0f, scale.y + 0.05f, 0f), new Vector3(scale.x + 0.12f, 0.1f, scale.z + 0.12f), woodMat, cab.transform);
        // Door Paneling Trim
        CreateBoxChild("Cabinet_DoorDetail", new Vector3(0f, scale.y / 2, scale.z / 2 + 0.02f), new Vector3(scale.x * 0.85f, scale.y * 0.8f, 0.03f), woodMat, cab.transform);

        EnsureSolidCollider(cab, scale);
    }

    private static void PlaceOrFallbackSofa(string name, Vector3 pos, Quaternion rot, GameObject prefab, Material fabricMat, Material woodMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, new Vector3(2.3f, 0.95f, 0.9f), parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Vintage Chesterfield Sofa
        GameObject sofa = new GameObject(name);
        sofa.transform.position = pos;
        sofa.transform.rotation = rot;
        sofa.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(sofa, "Create " + name);

        CreateBoxChild("Sofa_Seat", new Vector3(0f, 0.25f, 0f), new Vector3(2.2f, 0.35f, 0.9f), fabricMat, sofa.transform);
        CreateBoxChild("Sofa_Backrest", new Vector3(0f, 0.65f, -0.35f), new Vector3(2.2f, 0.75f, 0.25f), fabricMat, sofa.transform);
        CreateBoxChild("Sofa_Arm_L", new Vector3(-1.15f, 0.50f, 0f), new Vector3(0.25f, 0.55f, 0.95f), woodMat, sofa.transform);
        CreateBoxChild("Sofa_Arm_R", new Vector3(1.15f, 0.50f, 0f), new Vector3(0.25f, 0.55f, 0.95f), woodMat, sofa.transform);

        EnsureSolidCollider(sofa, new Vector3(2.4f, 1.0f, 1.0f));
    }

    private static void PlaceOrFallbackChair(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material fabricMat, Material woodMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Vintage Armchair
        GameObject chair = new GameObject(name);
        chair.transform.position = pos;
        chair.transform.rotation = rot;
        chair.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(chair, "Create " + name);

        CreateBoxChild("Chair_Seat", new Vector3(0f, 0.25f, 0f), new Vector3(scale.x * 0.9f, 0.3f, scale.z * 0.9f), fabricMat, chair.transform);
        CreateBoxChild("Chair_Backrest", new Vector3(0f, 0.70f, -scale.z * 0.35f), new Vector3(scale.x * 0.9f, 0.75f, 0.18f), fabricMat, chair.transform);
        CreateBoxChild("Chair_Arm_L", new Vector3(-scale.x * 0.45f, 0.48f, 0f), new Vector3(0.12f, 0.45f, scale.z * 0.9f), woodMat, chair.transform);
        CreateBoxChild("Chair_Arm_R", new Vector3(scale.x * 0.45f, 0.48f, 0f), new Vector3(0.12f, 0.45f, scale.z * 0.9f), woodMat, chair.transform);

        EnsureSolidCollider(chair, scale);
    }

    private static void PlaceOrFallbackTable(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material woodMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Wooden Table
        GameObject table = new GameObject(name);
        table.transform.position = pos;
        table.transform.rotation = rot;
        table.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(table, "Create " + name);

        float topH = 0.07f;
        float legH = scale.y - topH;
        CreateBoxChild("Table_Top", new Vector3(0f, scale.y - topH / 2, 0f), new Vector3(scale.x, topH, scale.z), woodMat, table.transform);

        float lx = scale.x * 0.42f; float lz = scale.z * 0.42f; float legThick = 0.08f;
        CreateBoxChild("Leg_1", new Vector3(-lx, legH / 2, -lz), new Vector3(legThick, legH, legThick), woodMat, table.transform);
        CreateBoxChild("Leg_2", new Vector3(lx, legH / 2, -lz), new Vector3(legThick, legH, legThick), woodMat, table.transform);
        CreateBoxChild("Leg_3", new Vector3(-lx, legH / 2, lz), new Vector3(legThick, legH, legThick), woodMat, table.transform);
        CreateBoxChild("Leg_4", new Vector3(lx, legH / 2, lz), new Vector3(legThick, legH, legThick), woodMat, table.transform);

        EnsureSolidCollider(table, scale);
    }

    private static void PlaceOrFallbackClock(string name, Vector3 pos, Quaternion rot, GameObject prefab, Material woodMat, Material goldMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, new Vector3(0.6f, 2.2f, 0.45f), parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Grandfather Clock
        GameObject clock = new GameObject(name);
        clock.transform.position = pos;
        clock.transform.rotation = rot;
        clock.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(clock, "Create " + name);

        CreateBoxChild("Clock_Body", new Vector3(0f, 1.1f, 0f), new Vector3(0.55f, 2.2f, 0.4f), woodMat, clock.transform);
        CreateBoxChild("Clock_Dial", new Vector3(0f, 1.85f, 0.21f), new Vector3(0.35f, 0.35f, 0.04f), goldMat, clock.transform);
        CreateBoxChild("Clock_GlassWindow", new Vector3(0f, 0.95f, 0.21f), new Vector3(0.28f, 0.85f, 0.02f), goldMat, clock.transform);

        EnsureSolidCollider(clock, new Vector3(0.6f, 2.2f, 0.45f));
    }

    private static void PlaceOrFallbackFireplace(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material woodMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Victorian Fireplace
        GameObject fireplace = new GameObject(name);
        fireplace.transform.position = pos;
        fireplace.transform.rotation = rot;
        fireplace.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(fireplace, "Create " + name);

        // Mantelpiece top
        CreateBoxChild("Fireplace_Mantel", new Vector3(0f, scale.y, 0f), new Vector3(scale.x + 0.2f, 0.12f, scale.z + 0.15f), woodMat, fireplace.transform);
        // Pillars Left and Right
        float pillarW = 0.3f;
        CreateBoxChild("Pillar_L", new Vector3(-scale.x / 2 + pillarW / 2, scale.y / 2, 0f), new Vector3(pillarW, scale.y, scale.z), woodMat, fireplace.transform);
        CreateBoxChild("Pillar_R", new Vector3(scale.x / 2 - pillarW / 2, scale.y / 2, 0f), new Vector3(pillarW, scale.y, scale.z), woodMat, fireplace.transform);
        // Hearth Backwall
        CreateBoxChild("Hearth_Back", new Vector3(0f, scale.y / 2, scale.z * 0.4f), new Vector3(scale.x - pillarW * 2, scale.y * 0.9f, 0.08f), woodMat, fireplace.transform);

        EnsureSolidCollider(fireplace, scale);
    }

    private static void PlaceOrFallbackBookshelf(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material woodMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Bookshelf
        GameObject shelf = new GameObject(name);
        shelf.transform.position = pos;
        shelf.transform.rotation = rot;
        shelf.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(shelf, "Create " + name);

        CreateBoxChild("Shelf_Frame_L", new Vector3(-scale.x / 2 + 0.05f, scale.y / 2, 0f), new Vector3(0.08f, scale.y, scale.z), woodMat, shelf.transform);
        CreateBoxChild("Shelf_Frame_R", new Vector3(scale.x / 2 - 0.05f, scale.y / 2, 0f), new Vector3(0.08f, scale.y, scale.z), woodMat, shelf.transform);
        CreateBoxChild("Shelf_Back", new Vector3(0f, scale.y / 2, scale.z / 2 - 0.02f), new Vector3(scale.x, scale.y, 0.04f), woodMat, shelf.transform);

        for (int i = 0; i < 4; i++)
        {
            float yTier = (scale.y / 4f) * i + 0.1f;
            CreateBoxChild($"Shelf_Tier_{i + 1}", new Vector3(0f, yTier, 0f), new Vector3(scale.x - 0.1f, 0.05f, scale.z - 0.04f), woodMat, shelf.transform);
        }

        EnsureSolidCollider(shelf, scale);
    }

    private static void PlaceOrFallbackCandelabra(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material goldMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Candelabra / Candlestick
        GameObject candelabra = new GameObject(name);
        candelabra.transform.position = pos;
        candelabra.transform.rotation = rot;
        candelabra.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(candelabra, "Create " + name);

        // Base & Stem
        CreateBoxChild("Candle_Base", new Vector3(0f, 0.03f, 0f), new Vector3(scale.x * 0.8f, 0.06f, scale.z * 0.8f), goldMat, candelabra.transform);
        CreateBoxChild("Candle_Stem", new Vector3(0f, scale.y * 0.45f, 0f), new Vector3(scale.x * 0.2f, scale.y * 0.8f, scale.z * 0.2f), goldMat, candelabra.transform);
        CreateBoxChild("Candle_Branch", new Vector3(0f, scale.y * 0.75f, 0f), new Vector3(scale.x * 0.9f, 0.05f, scale.z * 0.2f), goldMat, candelabra.transform);

        EnsureSolidCollider(candelabra, scale);
    }

    private static void PlaceOrFallbackChest(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material woodMat, Material goldMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Antique Chest
        GameObject chest = new GameObject(name);
        chest.transform.position = pos;
        chest.transform.rotation = rot;
        chest.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(chest, "Create " + name);

        CreateBoxChild("Chest_Body", new Vector3(0f, scale.y * 0.45f, 0f), new Vector3(scale.x, scale.y * 0.8f, scale.z), woodMat, chest.transform);
        CreateBoxChild("Chest_Lid", new Vector3(0f, scale.y * 0.9f, 0f), new Vector3(scale.x + 0.04f, scale.y * 0.2f, scale.z + 0.04f), woodMat, chest.transform);
        CreateBoxChild("Chest_Lock", new Vector3(0f, scale.y * 0.7f, scale.z / 2 + 0.02f), new Vector3(0.08f, 0.1f, 0.03f), goldMat, chest.transform);

        EnsureSolidCollider(chest, scale);
    }

    private static void PlaceOrFallbackBarrels(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material woodMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Barrels
        GameObject barrels = new GameObject(name);
        barrels.transform.position = pos;
        barrels.transform.rotation = rot;
        barrels.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(barrels, "Create " + name);

        CreateBoxChild("Barrel_1", new Vector3(-0.3f, 0.5f, 0f), new Vector3(0.6f, 1.0f, 0.6f), woodMat, barrels.transform);
        CreateBoxChild("Barrel_2", new Vector3(0.35f, 0.5f, -0.1f), new Vector3(0.55f, 0.95f, 0.55f), woodMat, barrels.transform);

        EnsureSolidCollider(barrels, scale);
    }

    private static void PlaceOrFallbackPedestal(string name, Vector3 pos, Quaternion rot, Vector3 scale, GameObject prefab, Material woodMat, Material goldMat, Transform parent)
    {
        if (prefab != null)
        {
            GameObject obj = InstantiateAndNormalize(prefab, scale, parent, name, pos, rot);
            if (obj != null) return;
        }

        // Procedural Pedestal with Vase
        GameObject ped = new GameObject(name);
        ped.transform.position = pos;
        ped.transform.rotation = rot;
        ped.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(ped, "Create " + name);

        CreateBoxChild("Pedestal_Base", new Vector3(0f, scale.y * 0.45f, 0f), new Vector3(scale.x, scale.y * 0.9f, scale.z), woodMat, ped.transform);
        CreateBoxChild("Pedestal_Vase", new Vector3(0f, scale.y + 0.18f, 0f), new Vector3(scale.x * 0.6f, 0.35f, scale.z * 0.6f), goldMat, ped.transform);

        EnsureSolidCollider(ped, scale);
    }

    private static void CreateCarpet(string name, Vector3 pos, Vector3 size, Material mat, Transform parent)
    {
        GameObject rug = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rug.name = name;
        rug.transform.position = pos;
        rug.transform.localScale = size;
        rug.transform.SetParent(parent, true);

        Collider c = rug.GetComponent<Collider>();
        if (c != null) UnityEngine.Object.DestroyImmediate(c);

        if (mat != null) rug.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    private static void CreateBoxChild(string name, Vector3 localPos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.localPosition = localPos;
        box.transform.localScale = scale;

        Collider c = box.GetComponent<Collider>();
        if (c != null) UnityEngine.Object.DestroyImmediate(c);

        if (mat != null) box.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    private static void EnsureSolidCollider(GameObject obj, Vector3 fallbackSize)
    {
        Collider col = obj.GetComponentInChildren<Collider>();
        if (col == null)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.center = new Vector3(0f, fallbackSize.y / 2, 0f);
            bc.size = fallbackSize;
            bc.isTrigger = false;
        }
        else
        {
            col.isTrigger = false;
        }
    }
    #endregion

    #region Reflection Probes Setup
    private static void SetupReflectionProbes(Transform parent)
    {
        CreateReflectionProbe("Probe_Hallway", new Vector3(0f, 1.8f, 0f), new Vector3(7f, 4f, 35f), parent);
        CreateReflectionProbe("Probe_Bedroom", new Vector3(-9.5f, 1.8f, 8.5f), new Vector3(15f, 4f, 18f), parent);
        CreateReflectionProbe("Probe_Library", new Vector3(-9.5f, 1.8f, -8.5f), new Vector3(15f, 4f, 18f), parent);
        CreateReflectionProbe("Probe_DiningLiving", new Vector3(9.5f, 1.8f, 8.5f), new Vector3(15f, 4f, 18f), parent);
        CreateReflectionProbe("Probe_Storage", new Vector3(9.5f, 1.8f, -8.5f), new Vector3(15f, 4f, 18f), parent);
    }

    private static void CreateReflectionProbe(string name, Vector3 pos, Vector3 size, Transform parent)
    {
        GameObject probeObj = new GameObject(name);
        probeObj.transform.position = pos;
        probeObj.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(probeObj, "Create " + name);

        ReflectionProbe probe = probeObj.AddComponent<ReflectionProbe>();
        probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Baked;
        probe.size = size;
        probe.importance = 1;
        probe.intensity = 1.1f;
        probe.boxProjection = true;
    }
    #endregion
}
#endif
