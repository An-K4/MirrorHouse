#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class DirectAssetDecorator
{
    private const string FurnitureRootName = "--- MANSION_FURNITURE_DECORATIONS ---";

    static DirectAssetDecorator()
    {
        EditorApplication.delayCall += ExecuteDirectDecoration;
    }

    [MenuItem("MirrorHouse/Fix All Purple Materials & Decorate Mansion (Direct)", false, 20)]
    public static void ManualTrigger()
    {
        ExecuteDirectDecoration();
    }

    private static void ExecuteDirectDecoration()
    {
        try
        {
            Debug.Log("[DirectAssetDecorator] 🚀 BẮT ĐẦU SỬA TRIỆT ĐỂ LỖI MÀU TÍM (PURPLE SHADERS) VÀ BỐ TRÍ NỘI THẤT THẬT...");

            // =========================================================================
            // GIAI ĐOẠN 1: SỬA TẤT CẢ VẬT LIỆU (MATERIALS) SANG URP LIT CHUẨN CÓ TEXTURE THẬT
            // =========================================================================
            int convertedMatCount = FixAllMaterialsToURP();

            // =========================================================================
            // GIAI ĐOẠN 2: MỞ SCENE LEVEL_MAIN
            // =========================================================================
            var activeScene = EditorSceneManager.GetActiveScene();
            string targetScenePath = "Assets/Scenes/Level_Main.unity";
            if (activeScene.path != targetScenePath)
            {
                if (File.Exists(targetScenePath))
                {
                    activeScene = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);
                }
            }

            if (activeScene.path != targetScenePath)
            {
                Debug.LogWarning("[DirectAssetDecorator] Không thể mở Level_Main.unity");
                return;
            }

            // =========================================================================
            // GIAI ĐOẠN 3: TẢI HOẶC CHUẨN BỊ CÁC ASSETS ĐỒ VẬT THẬT
            // =========================================================================
            // Standard Themed Materials
            Material darkWood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Mirror_Frame_DarkWood.mat");
            Material velvet = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Fabric_Velvet.mat");
            Material gold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Mirror_Frame_Gold.mat");
            Material tainted = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Mirror_Tainted.mat");
            Material altarStone = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Altar_Stone.mat");

            // Prefabs & Models
            // Beds
            GameObject bedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/Victorian_Bed.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Furniture/VictorianBed/victorian-bed/source/unzipped/Victorian Bed.fbx")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/MasterBed.obj");

            // Sofas
            GameObject sofaBig = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/sofa_big.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/3Seat.prefab");
            GameObject sofaSmall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/sofa_small.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/DoubleSeat.prefab");
            GameObject couch = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/Couch.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/3Seat2.prefab");

            // Chairs & Armchairs
            GameObject chairClassic = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/chair.prefab");
            GameObject armchair1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/Fotel.prefab");
            GameObject armchair2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/Fotel2.prefab");
            GameObject armchair3 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/Fotel3.prefab");
            GameObject armchair4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/Fotel4.prefab");

            // Cabinets & Wardrobes
            GameObject oldCabinet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Old Cabinet/Prefabs/SM_cabinet.prefab");
            GameObject bigCloset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/BigCloset.prefab");
            GameObject closet2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/closet2.prefab");
            GameObject closet3 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/Closet3.prefab");
            GameObject smallCloset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/SmallCloset.prefab");
            GameObject vanityComode = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/MirrorComode.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/MirrorComode2.prefab");

            // Tables & Desks
            GameObject diningTableSet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/Dining_Table_Set.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Furniture/DiningTable/source/Dining table.fbx");
            GameObject vintageDesk = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/Vintage_Desk.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/VintageDesk.obj");
            GameObject roundTable = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/RoundTable.prefab");
            GameObject metalTable = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Furniture/Prefabs/MetalTable.prefab");
            GameObject teaTable = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/tea_table 1.prefab")
                ?? roundTable;
            GameObject tvDesk = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/tv_desk.prefab")
                ?? vintageDesk;

            // Bookshelves
            GameObject bookshelfZNS = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/bookshelf.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Bookshelf.obj");

            // Altar, Safe, Barrels, Crates
            GameObject ritualAltar = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/Ritual_Altar.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/RitualAltar.obj");
            GameObject safeBox = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/Safe.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/SafeBox.obj");
            GameObject barrel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/Barrel.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Props/Storage_Barrel.prefab");
            GameObject crate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/StorageCrate.obj");

            // Lamps, Candles & Clocks
            GameObject antiqueLight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/antique_light.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Candelabra.obj");
            GameObject lightTall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/light_tall.prefab")
                ?? antiqueLight;
            GameObject candelabra = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Candelabra.obj")
                ?? antiqueLight;
            GameObject clockWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/clock_wall.prefab");

            // Props & Decors
            GameObject gramophone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/gramophone.prefab");
            GameObject vase1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/vase_1.prefab");
            GameObject vase2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/vase_2.prefab");
            GameObject painting1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/painting_1.prefab");
            GameObject painting2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/painting_2.prefab");
            GameObject painting3 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/painting_3.prefab");
            GameObject book1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/book_1.prefab");
            GameObject book2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/book_2.prefab");

            // Carpets
            GameObject carpet1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/carpet_1.prefab");
            GameObject carpet2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/carpet_2.prefab");
            GameObject carpet3 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/ZNS3D/Prefabs/carpet_3.prefab");

            // =========================================================================
            // GIAI ĐOẠN 4: DỌN DẸP ROOT CŨ & ĐẶT NỘI THẤT MỚI
            // =========================================================================
            GameObject oldRoot = GameObject.Find(FurnitureRootName);
            if (oldRoot != null) Undo.DestroyObjectImmediate(oldRoot);

            GameObject newRoot = new GameObject(FurnitureRootName);
            Undo.RegisterCreatedObjectUndo(newRoot, "Create " + FurnitureRootName);

            int placedCount = 0;

            // -------------------------------------------------------------------------
            // 1. PHÒNG NGỦ EMILY (Emily's Bedroom - Tây Bắc)
            // Cửa sang Thư viện: X = -9.5, Z = 0 (trống > 2.5m). Cửa ra Hành lang: X = -2.7, Z = 6 (trống > 2.5m)
            // -------------------------------------------------------------------------
            // Giường ngủ hoàng gia Victoria (sát tường Bắc Z = 13.5m)
            PlaceItemDirect("Prop_Bedroom_VictorianBed", new Vector3(-12.5f, 0f, 13.5f), Quaternion.Euler(0f, 0f, 0f), new Vector3(2.4f, 2.0f, 2.6f), bedPrefab, darkWood, newRoot.transform);
            placedCount++;

            // 2 Bàn đầu giường hai bên giường ngủ
            PlaceItemDirect("Prop_Bedroom_Nightstand_Left", new Vector3(-15.0f, 0f, 13.5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(0.7f, 0.75f, 0.7f), metalTable ?? roundTable, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Bedroom_Nightstand_Right", new Vector3(-10.0f, 0f, 13.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(0.7f, 0.75f, 0.7f), metalTable ?? roundTable, darkWood, newRoot.transform);
            placedCount += 2;

            // Chân nến / đèn ngủ cổ kính trên 2 bàn đầu giường
            PlaceItemDirect("Prop_Bedroom_Candle_Left", new Vector3(-15.0f, 0.75f, 13.5f), Quaternion.identity, new Vector3(0.35f, 0.5f, 0.35f), antiqueLight ?? candelabra, gold, newRoot.transform);
            PlaceItemDirect("Prop_Bedroom_Candle_Right", new Vector3(-10.0f, 0.75f, 13.5f), Quaternion.identity, new Vector3(0.35f, 0.5f, 0.35f), antiqueLight ?? candelabra, gold, newRoot.transform);
            placedCount += 2;

            // Tủ quần áo cổ Emily sát tường Tây (X = -15.8m, Z = 7.5m - cách xa cả 2 cửa)
            PlaceItemDirect("Prop_Cabinet_Bedroom_Emily", new Vector3(-15.8f, 0f, 7.5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.6f, 2.4f, 0.8f), oldCabinet ?? bigCloset, darkWood, newRoot.transform);
            placedCount++;

            // Bàn trang điểm có gương của Emily (sát vách Bắc X = -6.5f, Z = 15.5f)
            PlaceItemDirect("Prop_Bedroom_VanityComode", new Vector3(-6.5f, 0f, 15.5f), Quaternion.Euler(0f, 180f, 0f), new Vector3(1.4f, 2.1f, 0.7f), vanityComode ?? closet2, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Bedroom_VanityChair", new Vector3(-6.5f, 0f, 14.3f), Quaternion.Euler(0f, 0f, 0f), new Vector3(0.65f, 0.95f, 0.65f), chairClassic, velvet, newRoot.transform);
            placedCount += 2;

            // Ghế bành quý tộc bọc nhung góc thư giãn phòng ngủ
            PlaceItemDirect("Prop_Bedroom_VelvetArmchair", new Vector3(-4.5f, 0f, 11.5f), Quaternion.Euler(0f, -120f, 0f), new Vector3(1.0f, 1.1f, 0.95f), armchair2 ?? armchair1, velvet, newRoot.transform);
            PlaceItemDirect("Prop_Bedroom_TeaTable", new Vector3(-4.5f, 0f, 10.0f), Quaternion.identity, new Vector3(0.75f, 0.6f, 0.75f), roundTable ?? teaTable, darkWood, newRoot.transform);
            placedCount += 2;

            // Rương cổ chân giường
            PlaceItemDirect("Prop_Bedroom_Chest", new Vector3(-12.5f, 0f, 11.2f), Quaternion.identity, new Vector3(1.2f, 0.65f, 0.7f), crate, darkWood, newRoot.transform);
            placedCount++;

            // Thảm phòng ngủ lớn
            PlaceCarpet("Prop_Bedroom_Carpet", new Vector3(-12.5f, 0.015f, 13.0f), new Vector3(5.0f, 0.02f, 4.2f), carpet2, velvet, newRoot.transform);
            placedCount++;

            // -------------------------------------------------------------------------
            // 2. THƯ VIỆN CỔ HUYỀN BÍ (Occult Library - Tây Nam)
            // Cửa sang Phòng ngủ: X = -9.5, Z = 0 (trống > 2.5m). Cửa ra Hành lang: X = -2.7, Z = -6 (trống > 2.5m)
            // -------------------------------------------------------------------------
            // Kệ sách cổ kính lớn dọc vách Tây Thư viện
            PlaceItemDirect("Prop_Library_Bookshelf_West1", new Vector3(-16.0f, 0f, -6.5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(2.0f, 2.5f, 0.65f), bookshelfZNS, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Library_Bookshelf_West2", new Vector3(-16.0f, 0f, -11.5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(2.0f, 2.5f, 0.65f), bookshelfZNS, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Library_Bookshelf_East", new Vector3(-3.2f, 0f, -11.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.8f, 2.5f, 0.65f), bookshelfZNS, darkWood, newRoot.transform);
            placedCount += 3;

            // Bàn làm việc nghiên cứu cổ trung tâm thư viện
            PlaceItemDirect("Prop_Library_StudyDesk", new Vector3(-9.5f, 0f, -9.5f), Quaternion.Euler(0f, 180f, 0f), new Vector3(1.8f, 0.85f, 1.1f), vintageDesk, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Library_StudyChair", new Vector3(-9.5f, 0f, -8.3f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.7f, 1.0f, 0.7f), chairClassic, velvet, newRoot.transform);
            placedCount += 2;

            // Sách cổ trên bàn học
            PlaceItemDirect("Prop_Library_Book1", new Vector3(-9.1f, 0.86f, -9.4f), Quaternion.Euler(0f, 25f, 0f), new Vector3(0.35f, 0.1f, 0.28f), book1, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Library_Book2", new Vector3(-9.8f, 0.86f, -9.5f), Quaternion.Euler(0f, -15f, 0f), new Vector3(0.32f, 0.08f, 0.25f), book2, darkWood, newRoot.transform);
            placedCount += 2;

            // Tủ cấm thư Grimoire & Tủ tài liệu sát vách Nam (Z = -15.8m)
            PlaceItemDirect("Prop_Cabinet_Library_Grimoire", new Vector3(-14.5f, 0f, -15.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(1.6f, 2.4f, 0.8f), oldCabinet ?? closet2, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Cabinet_Library_Archives", new Vector3(-5.0f, 0f, -15.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(1.6f, 2.4f, 0.8f), closet3 ?? bigCloset, darkWood, newRoot.transform);
            placedCount += 2;

            // Góc đọc sách thư viện với ghế bành và máy hát đĩa cổ
            PlaceItemDirect("Prop_Library_ReadingArmchair", new Vector3(-14.0f, 0f, -14.0f), Quaternion.Euler(0f, 45f, 0f), new Vector3(1.0f, 1.1f, 0.95f), armchair3 ?? armchair1, velvet, newRoot.transform);
            PlaceItemDirect("Prop_Library_TeaTable", new Vector3(-12.5f, 0f, -13.0f), Quaternion.identity, new Vector3(0.75f, 0.6f, 0.75f), teaTable ?? roundTable, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Library_Gramophone", new Vector3(-12.5f, 0.6f, -13.0f), Quaternion.Euler(0f, -30f, 0f), new Vector3(0.45f, 0.65f, 0.45f), gramophone, gold, newRoot.transform);
            placedCount += 3;

            // Đồng hồ treo tường cổ thư viện
            PlaceWallDecor("Prop_Library_WallClock", new Vector3(-9.5f, 2.4f, -16.7f), Quaternion.Euler(0f, 0f, 0f), new Vector3(0.55f, 0.9f, 0.2f), clockWall, darkWood, newRoot.transform);
            placedCount++;

            // Thảm thư viện
            PlaceCarpet("Prop_Library_Carpet", new Vector3(-9.5f, 0.015f, -9.5f), new Vector3(4.8f, 0.02f, 4.0f), carpet3, darkWood, newRoot.transform);
            placedCount++;

            // -------------------------------------------------------------------------
            // 3. PHÒNG KHÁCH & BÀN ĂN (Vintage Living & Dining - Đông Bắc)
            // Cửa sang Nhà kho: X = 9.5, Z = 0 (trống > 2.5m). Cửa ra Hành lang: X = 2.7, Z = 6 (trống > 2.5m)
            // -------------------------------------------------------------------------
            // Bàn ăn dài quý tộc tại X = 9.5f, Z = 8.5f (cách cửa Z = 0 tới 8.5m)
            PlaceItemDirect("Prop_Dining_TableSet", new Vector3(9.5f, 0f, 8.5f), Quaternion.identity, new Vector3(2.4f, 1.0f, 1.4f), diningTableSet ?? vintageDesk, darkWood, newRoot.transform);
            placedCount++;

            // Ghế ăn xung quanh bàn
            for (int i = 0; i < 4; i++)
            {
                Vector3 cPos = (i == 0) ? new Vector3(8.3f, 0f, 9.6f) :
                               (i == 1) ? new Vector3(10.7f, 0f, 9.6f) :
                               (i == 2) ? new Vector3(8.3f, 0f, 7.4f) : new Vector3(10.7f, 0f, 7.4f);
                Quaternion cRot = (i < 2) ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(0f, 0f, 0f);
                PlaceItemDirect($"Prop_Dining_Chair_{i + 1}", cPos, cRot, new Vector3(0.65f, 1.0f, 0.65f), chairClassic, velvet, newRoot.transform);
                placedCount++;
            }

            // Bộ ghế Sofa phòng khách lớn (sát vách Tây khu khách X = 7.0f, Z = 14.0f)
            PlaceItemDirect("Prop_Living_SofaBig", new Vector3(7.0f, 0f, 14.0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(2.4f, 1.05f, 1.0f), sofaBig ?? couch, velvet, newRoot.transform);
            // Ghế sofa đơn đối diện
            PlaceItemDirect("Prop_Living_SofaSmall", new Vector3(13.0f, 0f, 14.0f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.1f, 1.05f, 1.0f), sofaSmall ?? armchair4, velvet, newRoot.transform);
            // Bàn trà phòng khách giữa bộ sofa
            PlaceItemDirect("Prop_Living_CoffeeTable", new Vector3(10.0f, 0f, 14.0f), Quaternion.identity, new Vector3(1.5f, 0.55f, 0.85f), teaTable ?? metalTable, darkWood, newRoot.transform);
            placedCount += 3;

            // Bình hoa trên bàn trà phòng khách
            PlaceItemDirect("Prop_Living_Vase", new Vector3(10.0f, 0.55f, 14.0f), Quaternion.identity, new Vector3(0.25f, 0.45f, 0.25f), vase1, gold, newRoot.transform);
            placedCount++;

            // Tủ ly phòng ăn (Old Cabinet) sát vách Đông X = 16.0f, Z = 11.5f
            PlaceItemDirect("Prop_Cabinet_Dining_GlassCabinet", new Vector3(16.0f, 0f, 11.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.6f, 2.4f, 0.8f), oldCabinet, darkWood, newRoot.transform);
            // Tủ búp phê sát vách Bắc X = 5.0f, Z = 16.0f
            PlaceItemDirect("Prop_Cabinet_Dining_Buffet", new Vector3(5.0f, 0f, 16.0f), Quaternion.Euler(0f, 180f, 0f), new Vector3(1.8f, 2.2f, 0.8f), bigCloset ?? smallCloset, darkWood, newRoot.transform);
            placedCount += 2;

            // Đèn cây đứng góc phòng khách
            PlaceItemDirect("Prop_Living_FloorLamp", new Vector3(15.8f, 0f, 15.8f), Quaternion.Euler(0f, -135f, 0f), new Vector3(0.5f, 2.0f, 0.5f), lightTall ?? antiqueLight, gold, newRoot.transform);
            placedCount++;

            // Bàn đón tiếp sảnh ăn (gần vách ngăn X = 3.2f, Z = 4.0f)
            PlaceItemDirect("Prop_Dining_FoyerTable", new Vector3(3.2f, 0f, 4.0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.1f, 0.85f, 0.5f), tvDesk ?? metalTable, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Dining_FoyerVase", new Vector3(3.2f, 0.85f, 4.0f), Quaternion.identity, new Vector3(0.25f, 0.45f, 0.25f), vase2 ?? vase1, gold, newRoot.transform);
            placedCount += 2;

            // Tranh treo tường cổ điển
            PlaceWallDecor("Prop_Living_Painting1", new Vector3(10.0f, 2.2f, 16.7f), Quaternion.Euler(0f, 180f, 0f), new Vector3(1.6f, 1.1f, 0.1f), painting1, darkWood, newRoot.transform);
            PlaceWallDecor("Prop_Living_Painting2", new Vector3(16.7f, 2.2f, 7.5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.4f, 1.0f, 0.1f), painting2, darkWood, newRoot.transform);
            placedCount += 2;

            // Thảm phòng khách
            PlaceCarpet("Prop_Living_Carpet", new Vector3(10.0f, 0.015f, 14.0f), new Vector3(4.8f, 0.02f, 4.0f), carpet1, velvet, newRoot.transform);
            placedCount++;

            // -------------------------------------------------------------------------
            // 4. NHÀ KHO & PHÒNG TẾ LỄ (Storage & Ritual Chamber - Đông Nam)
            // Cửa sang Phòng ăn: X = 9.5, Z = 0 (trống > 2.5m). Cửa ra Hành lang: X = 2.7, Z = -6 (trống > 2.5m)
            // -------------------------------------------------------------------------
            // Bàn Tế Đàn Hắc Ám uy nghi (X = 9.5f, Z = -13.5f sát tường Nam)
            PlaceItemDirect("Prop_Ritual_Altar", new Vector3(9.5f, 0f, -13.5f), Quaternion.Euler(0f, 0f, 0f), new Vector3(2.2f, 1.3f, 1.4f), ritualAltar, altarStone, newRoot.transform);
            // 2 Chân nến nghi lễ hai bên Tế Đàn
            PlaceItemDirect("Prop_Ritual_Candle_Left", new Vector3(7.8f, 0f, -13.5f), Quaternion.identity, new Vector3(0.45f, 1.4f, 0.45f), candelabra ?? antiqueLight, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Ritual_Candle_Right", new Vector3(11.2f, 0f, -13.5f), Quaternion.identity, new Vector3(0.45f, 1.4f, 0.45f), candelabra ?? antiqueLight, darkWood, newRoot.transform);
            placedCount += 3;

            // Vòng phong ấn tế lễ trên sàn
            PlaceCarpet("Prop_Ritual_FloorDecal", new Vector3(9.5f, 0.015f, -11.5f), new Vector3(3.8f, 0.02f, 3.8f), null, tainted, newRoot.transform);
            placedCount++;

            // Két sắt kim loại bí mật góc Đông Nam X = 15.5f, Z = -15.5f
            PlaceItemDirect("Prop_Storage_SecretSafe", new Vector3(15.5f, 0f, -15.5f), Quaternion.Euler(0f, -45f, 0f), new Vector3(0.9f, 1.3f, 0.85f), safeBox, darkWood, newRoot.transform);
            placedCount++;

            // Tủ chứa cổ vật tế lễ sát vách Nam X = 4.0f, Z = -15.8f
            PlaceItemDirect("Prop_Cabinet_Storage_Relics", new Vector3(4.0f, 0f, -15.8f), Quaternion.Euler(0f, 0f, 0f), new Vector3(1.6f, 2.3f, 0.8f), oldCabinet ?? closet2, darkWood, newRoot.transform);
            placedCount++;

            // Thùng gỗ chứa đồ xếp tại các góc nhà kho
            PlaceItemDirect("Prop_Storage_Barrels_1", new Vector3(15.5f, 0f, -5.0f), Quaternion.identity, new Vector3(1.1f, 1.2f, 1.1f), barrel, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Storage_Barrels_2", new Vector3(15.5f, 0f, -10.5f), Quaternion.identity, new Vector3(1.1f, 1.2f, 1.1f), barrel, darkWood, newRoot.transform);
            placedCount += 2;

            // Hòm gỗ cũ
            PlaceItemDirect("Prop_Storage_Crates", new Vector3(3.8f, 0f, -4.5f), Quaternion.Euler(0f, 20f, 0f), new Vector3(1.2f, 0.9f, 0.9f), crate, darkWood, newRoot.transform);
            placedCount++;

            // Kệ để đồ sắt cũ sát vách Tây nhà kho X = 3.2f, Z = -10.0f
            PlaceItemDirect("Prop_Storage_Shelf", new Vector3(3.2f, 0f, -10.0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.8f, 2.3f, 0.6f), bookshelfZNS ?? closet3, darkWood, newRoot.transform);
            placedCount++;

            // -------------------------------------------------------------------------
            // 5. HÀNH LANG CHÍNH (Hall of Mirrors)
            // Giữ thông thoáng 100% trục giữa X: -1.5m đến +1.5m
            // -------------------------------------------------------------------------
            PlaceItemDirect("Prop_Hallway_Pedestal_West", new Vector3(-2.35f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(0.4f, 1.1f, 0.4f), metalTable ?? vase1, darkWood, newRoot.transform);
            PlaceItemDirect("Prop_Hallway_Pedestal_East", new Vector3(1.55f, 0f, 0f), Quaternion.Euler(0f, -90f, 0f), new Vector3(0.4f, 1.1f, 0.4f), metalTable ?? vase2, darkWood, newRoot.transform);
            placedCount += 2;

            // =========================================================================
            // GIAI ĐOẠN 5: KIỂM SOÁT VÀ SỬA TRIỆT ĐỂ RENDERERS TRÊN TẤT CẢ OBJECT ĐÃ ĐẶT
            // (ĐẢM BẢO 100% KHÔNG CÒN OBJECT NÀO BỊ MÀU TÍM)
            // =========================================================================
            int fixedRendererCount = ValidateAndFixAllRenderers(newRoot, darkWood, velvet, gold, altarStone);

            // =========================================================================
            // GIAI ĐOẠN 6: LƯU SCENE VĨNH VIỄN VÀ BÁO CÁO
            // =========================================================================
            EditorSceneManager.MarkSceneDirty(activeScene);
            bool saved = EditorSceneManager.SaveScene(activeScene);
            AssetDatabase.SaveAssets();

            string report = $"=== BÁO CÁO SỬA LỖI MÀU TÍM & NỘI THẤT LEVEL_MAIN ({DateTime.Now}) ===\n" +
                            $"- Số Material đã nâng cấp sang URP Lit: {convertedMatCount}\n" +
                            $"- Số đồ nội thất 3D thật đã bố trí trực tiếp: {placedCount}\n" +
                            $"- Số MeshRenderer đã kiểm tra và chuẩn hóa: {fixedRendererCount}\n" +
                            $"- Cửa kết nối (Bedroom-Library & Dining-Storage): HOÀN TOÀN THÔNG THOÁNG (> 7m)\n" +
                            $"- Scene Level_Main đã lưu vĩnh viễn: {saved}\n";

            File.WriteAllText("Assets/furniture_fix_report.txt", report);
            Debug.Log($"[DirectAssetDecorator] ✅ HOÀN TẤT THÀNH CÔNG!\n{report}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DirectAssetDecorator] ❌ Lỗi trong quá trình bố trí: {ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Quét và chuyển đổi toàn bộ material Built-in và HDRP sang URP Lit, nối texture tương ứng.
    /// </summary>
    public static int FixAllMaterialsToURP()
    {
        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            Debug.LogError("[DirectAssetDecorator] Không tìm thấy Shader 'Universal Render Pipeline/Lit'!");
            return 0;
        }

        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        int count = 0;
        List<string> logs = new List<string>();

        foreach (string guid in matGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("/TextMesh Pro/") || path.Contains("/Joystick Pack/")) continue;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            bool isBrokenOrIncompatible = (mat.shader == null) ||
                                          (mat.shader.name != "Universal Render Pipeline/Lit" &&
                                           mat.shader.name != "Universal Render Pipeline/Simple Lit" &&
                                           mat.shader.name != "Universal Render Pipeline/Unlit" &&
                                           mat.shader.name != "Universal Render Pipeline/Particles/Lit" &&
                                           !mat.shader.name.StartsWith("Universal Render Pipeline/"));

            if (isBrokenOrIncompatible)
            {
                // Thu thập texture hiện có trước khi đổi shader
                Texture baseMap = null;
                Texture bumpMap = null;
                Texture metallicMap = null;
                Texture aoMap = null;

                if (mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) baseMap = mat.GetTexture("_BaseMap");
                else if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null) baseMap = mat.GetTexture("_MainTex");
                else if (mat.HasProperty("_BaseColorMap") && mat.GetTexture("_BaseColorMap") != null) baseMap = mat.GetTexture("_BaseColorMap");

                if (mat.HasProperty("_BumpMap") && mat.GetTexture("_BumpMap") != null) bumpMap = mat.GetTexture("_BumpMap");
                else if (mat.HasProperty("_NormalMap") && mat.GetTexture("_NormalMap") != null) bumpMap = mat.GetTexture("_NormalMap");

                if (mat.HasProperty("_MetallicGlossMap") && mat.GetTexture("_MetallicGlossMap") != null) metallicMap = mat.GetTexture("_MetallicGlossMap");
                else if (mat.HasProperty("_MaskMap") && mat.GetTexture("_MaskMap") != null) metallicMap = mat.GetTexture("_MaskMap");

                if (mat.HasProperty("_OcclusionMap") && mat.GetTexture("_OcclusionMap") != null) aoMap = mat.GetTexture("_OcclusionMap");

                // Đổi sang URP Lit
                mat.shader = urpLitShader;

                // Tự động tìm texture phù hợp nếu chưa có
                string matDir = Path.GetDirectoryName(path).Replace("\\", "/");
                string matName = Path.GetFileNameWithoutExtension(path);

                if (baseMap == null)
                {
                    baseMap = FindTextureForMaterial(matDir, matName, TextureType.BaseColor);
                }
                if (bumpMap == null)
                {
                    bumpMap = FindTextureForMaterial(matDir, matName, TextureType.Normal);
                }
                if (metallicMap == null)
                {
                    metallicMap = FindTextureForMaterial(matDir, matName, TextureType.Metallic);
                }

                if (baseMap != null) mat.SetTexture("_BaseMap", baseMap);
                if (bumpMap != null)
                {
                    mat.SetTexture("_BumpMap", bumpMap);
                    mat.EnableKeyword("_NORMALMAP");
                }
                if (metallicMap != null)
                {
                    mat.SetTexture("_MetallicGlossMap", metallicMap);
                    mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                }
                if (aoMap != null)
                {
                    mat.SetTexture("_OcclusionMap", aoMap);
                    mat.EnableKeyword("_OCCLUSIONMAP");
                }

                mat.SetColor("_BaseColor", Color.white);
                mat.SetFloat("_Smoothness", 0.5f);
                mat.SetFloat("_Metallic", 0.1f);

                EditorUtility.SetDirty(mat);
                count++;
                logs.Add($"Converted to URP: {path} (Base: {baseMap?.name ?? "none"}, Normal: {bumpMap?.name ?? "none"})");
            }
        }

        AssetDatabase.SaveAssets();
        File.WriteAllLines("Assets/material_conversion_report.txt", logs);
        Debug.Log($"[DirectAssetDecorator] Đã chuyển đổi {count} vật liệu sang URP Lit thành công!");
        return count;
    }

    private enum TextureType { BaseColor, Normal, Metallic }

    private static Texture FindTextureForMaterial(string matDir, string matName, TextureType type)
    {
        // 1. Thư mục Textures lân cận
        string parentDir = Path.GetDirectoryName(matDir).Replace("\\", "/");
        string[] searchDirs = new string[]
        {
            matDir,
            $"{matDir}/Textures",
            $"{parentDir}/Textures",
            $"{parentDir}/textures"
        };

        string cleanName = matName.Replace("Mat_", "").Replace("mat_", "").Replace("Mat-", "").Replace("_BaseColor", "").Replace("M_", "");

        foreach (string dir in searchDirs)
        {
            if (!Directory.Exists(dir)) continue;

            string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)) continue;
                string fname = Path.GetFileNameWithoutExtension(file);

                bool nameMatches = fname.IndexOf(cleanName, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                   cleanName.IndexOf(fname, StringComparison.OrdinalIgnoreCase) >= 0;

                if (!nameMatches) continue;

                string lower = fname.ToLower();
                if (type == TextureType.Normal && (lower.Contains("normal") || lower.EndsWith("_n") || lower.Contains("bump")))
                {
                    return AssetDatabase.LoadAssetAtPath<Texture>(file.Replace("\\", "/"));
                }
                else if (type == TextureType.Metallic && (lower.Contains("metallic") || lower.Contains("mask") || lower.EndsWith("_m")))
                {
                    return AssetDatabase.LoadAssetAtPath<Texture>(file.Replace("\\", "/"));
                }
                else if (type == TextureType.BaseColor && (lower.Contains("basecolor") || lower.Contains("albedo") || lower.Contains("diffuse") || lower.EndsWith("_bc") || lower.Contains("colour")))
                {
                    return AssetDatabase.LoadAssetAtPath<Texture>(file.Replace("\\", "/"));
                }
            }
        }

        return null;
    }

    private static void PlaceItemDirect(string name, Vector3 pos, Quaternion rot, Vector3 targetSize, GameObject prefabOrModel, Material fallbackMat, Transform parent)
    {
        if (prefabOrModel != null)
        {
            GameObject obj;
            if (PrefabUtility.IsPartOfPrefabAsset(prefabOrModel))
            {
                obj = (GameObject)PrefabUtility.InstantiatePrefab(prefabOrModel);
            }
            else
            {
                obj = UnityEngine.Object.Instantiate(prefabOrModel);
            }

            if (obj != null)
            {
                obj.name = name;
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.transform.SetParent(parent, true);
                obj.transform.localScale = Vector3.one;

                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    Bounds b = renderers[0].bounds;
                    for (int i = 1; i < renderers.Length; i++)
                    {
                        b.Encapsulate(renderers[i].bounds);
                    }

                    Vector3 curSize = b.size;
                    if (curSize.y > 0.05f && targetSize.y > 0.05f)
                    {
                        float scaleRatio = targetSize.y / curSize.y;
                        obj.transform.localScale = Vector3.one * scaleRatio;

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

                EnsureCollider(obj, targetSize);
                Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
                return;
            }
        }

        // Fallback procedural
        GameObject fb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fb.name = name;
        fb.transform.position = new Vector3(pos.x, pos.y + targetSize.y / 2, pos.z);
        fb.transform.rotation = rot;
        fb.transform.localScale = targetSize;
        fb.transform.SetParent(parent, true);
        if (fallbackMat != null) fb.GetComponent<MeshRenderer>().sharedMaterial = fallbackMat;
        Undo.RegisterCreatedObjectUndo(fb, "Create " + name);
    }

    private static void PlaceCarpet(string name, Vector3 pos, Vector3 targetSize, GameObject carpetPrefab, Material fallbackMat, Transform parent)
    {
        if (carpetPrefab != null)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(carpetPrefab);
            if (obj != null)
            {
                obj.name = name;
                obj.transform.position = pos;
                obj.transform.rotation = Quaternion.identity;
                obj.transform.SetParent(parent, true);
                obj.transform.localScale = new Vector3(targetSize.x / 4.0f, 1f, targetSize.z / 3.0f);

                // Xóa mọi collider trên thảm để người chơi di chuyển êm ái
                Collider[] cols = obj.GetComponentsInChildren<Collider>();
                foreach (var c in cols) UnityEngine.Object.DestroyImmediate(c);

                Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
                return;
            }
        }

        // Procedural Carpet
        GameObject rug = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rug.name = name;
        rug.transform.position = pos;
        rug.transform.localScale = targetSize;
        rug.transform.SetParent(parent, true);

        Collider col = rug.GetComponent<Collider>();
        if (col != null) UnityEngine.Object.DestroyImmediate(col);

        if (fallbackMat != null) rug.GetComponent<MeshRenderer>().sharedMaterial = fallbackMat;
        Undo.RegisterCreatedObjectUndo(rug, "Create " + name);
    }

    private static void PlaceWallDecor(string name, Vector3 pos, Quaternion rot, Vector3 size, GameObject decorPrefab, Material fallbackMat, Transform parent)
    {
        if (decorPrefab != null)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(decorPrefab);
            if (obj != null)
            {
                obj.name = name;
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.transform.SetParent(parent, true);

                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    Bounds b = renderers[0].bounds;
                    for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
                    if (b.size.y > 0.05f)
                    {
                        float ratio = size.y / b.size.y;
                        obj.transform.localScale = Vector3.one * ratio;
                    }
                }

                Collider[] cols = obj.GetComponentsInChildren<Collider>();
                foreach (var c in cols) UnityEngine.Object.DestroyImmediate(c);

                Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
                return;
            }
        }

        // Procedural
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
        p.name = name;
        p.transform.position = pos;
        p.transform.rotation = rot;
        p.transform.localScale = size;
        p.transform.SetParent(parent, true);
        if (fallbackMat != null) p.GetComponent<MeshRenderer>().sharedMaterial = fallbackMat;
        Collider col = p.GetComponent<Collider>();
        if (col != null) UnityEngine.Object.DestroyImmediate(col);
        Undo.RegisterCreatedObjectUndo(p, "Create " + name);
    }

    private static int ValidateAndFixAllRenderers(GameObject root, Material woodMat, Material velvetMat, Material goldMat, Material stoneMat)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        int fixedCount = 0;

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.sharedMaterials;
            bool modified = false;

            for (int i = 0; i < mats.Length; i++)
            {
                Material m = mats[i];
                bool isBad = (m == null) ||
                             (m.shader == null) ||
                             (m.name.Contains("Default")) ||
                             (m.shader.name != "Universal Render Pipeline/Lit" &&
                              m.shader.name != "Universal Render Pipeline/Simple Lit" &&
                              m.shader.name != "Universal Render Pipeline/Unlit" &&
                              !m.shader.name.StartsWith("Universal Render Pipeline/"));

                if (isBad)
                {
                    string rName = r.gameObject.name.ToLower();
                    if (rName.Contains("candle") || rName.Contains("gold") || rName.Contains("light") || rName.Contains("brass"))
                    {
                        mats[i] = goldMat;
                    }
                    else if (rName.Contains("sofa") || rName.Contains("cushion") || rName.Contains("velvet") || rName.Contains("fabric") || rName.Contains("armchair"))
                    {
                        mats[i] = velvetMat;
                    }
                    else if (rName.Contains("altar") || rName.Contains("stone") || rName.Contains("rock"))
                    {
                        mats[i] = stoneMat;
                    }
                    else
                    {
                        mats[i] = woodMat;
                    }
                    modified = true;
                    fixedCount++;
                }
            }

            if (modified)
            {
                r.sharedMaterials = mats;
                EditorUtility.SetDirty(r);
            }
        }

        return fixedCount;
    }

    private static void EnsureCollider(GameObject obj, Vector3 fallbackSize)
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
}
#endif
