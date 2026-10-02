#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Collections.Generic;

public class World2SceneBuilder : EditorWindow
{
    [MenuItem("MirrorHouse/Build World 2 Complete Scene", false, 1)]
    public static void BuildCompleteWorld2Scene()
    {
        var currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.name != "World2_Prototype")
        {
            string scenePath = "Assets/Scenes/World2_Prototype.unity";
            if (File.Exists(scenePath))
            {
                currentScene = EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                currentScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        // 1. Clean up old root objects
        var rootObjects = currentScene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            Undo.DestroyObjectImmediate(obj);
        }

        // Ensure directories exist
        EnsureDirectories();

        // 2. Load Materials
        Material matFloor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor_Wood.mat");
        Material matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Wall_Dark.mat");
        Material matDoor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Door.mat");
        Material matDesk = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Desk.mat");
        Material matMirror = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Mirror.mat");
        Material matMonster = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Monster.mat");
        Material matSafe = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Safe.mat");
        Material matAltar = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Altar.mat");
        Material matDagger = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Dagger.mat");
        Material matBed = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_BedWood.mat");
        Material matDining = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Dining.mat");
        Material matBookshelf = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Bookshelf.mat");
        Material matBarrel = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Barrel.mat");
        Material matKey = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Imported/Mat_Imported_Key.mat");
        Material matPaper = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Clue_Paper.mat");

        Material defaultMat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
        if (matFloor == null) matFloor = defaultMat;
        if (matWall == null) matWall = defaultMat;

        // Lighting environment
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.15f, 0.13f, 0.12f, 1f);
        RenderSettings.ambientIntensity = 1.0f;
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.04f, 0.03f, 0.05f, 1f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.012f;

        // Global Moon Light
        GameObject globalLightGo = new GameObject("Moonlight_Global_Directional");
        Light globalLight = globalLightGo.AddComponent<Light>();
        globalLight.type = LightType.Directional;
        globalLight.color = new Color(0.35f, 0.42f, 0.58f);
        globalLight.intensity = 0.35f;
        globalLight.shadows = LightShadows.Soft;
        globalLightGo.transform.rotation = Quaternion.Euler(60f, -40f, 0f);

        // Helper: Create Box Wall
        System.Func<string, Vector3, Vector3, Material, GameObject> CreateBox = (name, pos, scale, mat) => {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.position = pos;
            box.transform.localScale = scale;
            if (mat != null) box.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return box;
        };

        // Helper: Create Point Light
        System.Func<string, Vector3, float, float, Color, GameObject> CreatePointLight = (name, pos, range, intensity, color) => {
            GameObject lightGo = new GameObject(name);
            lightGo.transform.position = pos;
            Light lt = lightGo.AddComponent<Light>();
            lt.type = LightType.Point;
            lt.range = range;
            lt.intensity = intensity;
            lt.color = color;
            lt.shadows = LightShadows.Soft;
            return lightGo;
        };

        // Helper: Create Standardized Prefab with Exact Physical Dimensions
        System.Func<string, string, float, Material, GameObject> CreateOrGetStandardizedPrefab = (modelSourcePath, prefabSavePath, targetHeight, overrideMat) => {
            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(modelSourcePath);
            if (modelAsset == null)
            {
                Debug.LogWarning($"[World2Builder] Không tìm thấy model tại: {modelSourcePath}");
                return null;
            }

            // Create temporary root
            GameObject tempRoot = new GameObject(Path.GetFileNameWithoutExtension(prefabSavePath));
            GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
            modelInstance.transform.SetParent(tempRoot.transform, false);
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localRotation = Quaternion.identity;
            modelInstance.transform.localScale = Vector3.one;

            // Measure unscaled bounds
            Renderer[] renderers = modelInstance.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }

                float currentHeight = bounds.size.y;
                float currentMax = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                float refSize = currentHeight > 0.05f ? currentHeight : currentMax;

                if (refSize > 0.001f && targetHeight > 0.005f)
                {
                    float factor = targetHeight / refSize;
                    modelInstance.transform.localScale = new Vector3(factor, factor, factor);
                }
            }

            // Apply override material if requested
            if (overrideMat != null)
            {
                foreach (var r in modelInstance.GetComponentsInChildren<MeshRenderer>())
                    r.sharedMaterial = overrideMat;
                foreach (var r in modelInstance.GetComponentsInChildren<SkinnedMeshRenderer>())
                    r.sharedMaterial = overrideMat;
            }

            // Add Collider if missing
            if (tempRoot.GetComponent<Collider>() == null && tempRoot.GetComponentInChildren<Collider>() == null)
            {
                tempRoot.AddComponent<BoxCollider>();
            }

            // Save Prefab
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(tempRoot, prefabSavePath);
            GameObject.DestroyImmediate(tempRoot);
            return savedPrefab;
        };

        // =================================================================
        // STANDARDIZED PREFABS GENERATION (1 Unity Unit = 1m)
        // =================================================================
        Debug.Log("<b>[World2Builder] Bắt đầu chuẩn hóa kích thước các Model 3D thành Prefab chuẩn...</b>");

        // Props
        GameObject prefabKey = CreateOrGetStandardizedPrefab("Assets/3D model/antique-key/source/key.glb", "Assets/Prefabs/Props/Key_Gold.prefab", 0.08f, matKey);
        GameObject prefabDagger = CreateOrGetStandardizedPrefab("Assets/3D model/ritual-dagger/source/Royal Dagger.fbx", "Assets/Prefabs/Props/Ritual_Dagger.prefab", 0.30f, matDagger);
        GameObject prefabSafe = CreateOrGetStandardizedPrefab("Assets/3D model/safe/source/Safe.fbx", "Assets/Prefabs/Props/Safe_Box.prefab", 0.55f, matSafe);
        GameObject prefabBarrel = CreateOrGetStandardizedPrefab("Assets/3D model/wooden-barrel-and-crate/source/unzipped/Barrel.fbx", "Assets/Prefabs/Props/Storage_Barrel.prefab", 0.80f, matBarrel);

        // Furniture
        GameObject prefabDoor = CreateOrGetStandardizedPrefab("Assets/3D model/victorian-door/source/Door.fbx", "Assets/Prefabs/Furniture/Victorian_Door.prefab", 2.15f, matDoor);
        GameObject prefabBed = CreateOrGetStandardizedPrefab("Assets/3D model/victorian-bed/source/unzipped/Victorian Bed.fbx", "Assets/Prefabs/Furniture/Victorian_Bed.prefab", 1.25f, matBed);
        GameObject prefabDesk = CreateOrGetStandardizedPrefab("Assets/3D model/antique-desk/source/model.fbx", "Assets/Prefabs/Furniture/Vintage_Desk.prefab", 0.78f, matDesk);
        GameObject prefabBookshelf = CreateOrGetStandardizedPrefab("Assets/3D model/victorian-bookshelf/source/victorian_bookshelf.glb", "Assets/Prefabs/Furniture/Victorian_Bookshelf.prefab", 2.05f, matBookshelf);
        GameObject prefabDining = CreateOrGetStandardizedPrefab("Assets/3D model/dining-table-and-chairs/source/Dining table.fbx", "Assets/Prefabs/Furniture/Dining_Table_Set.prefab", 0.78f, matDining);
        GameObject prefabAltar = CreateOrGetStandardizedPrefab("Assets/3D model/ritual-altar-game-asset/source/Altar_Low.fbx", "Assets/Prefabs/Furniture/Ritual_Altar.prefab", 0.85f, matAltar);
        GameObject prefabMirror = CreateOrGetStandardizedPrefab("Assets/3D model/gothic-mirror/source/mirror.fbx", "Assets/Prefabs/Furniture/Grand_Mirror.prefab", 2.20f, matMirror);

        // Characters
        GameObject prefabMonster = CreateOrGetStandardizedPrefab("Assets/3D model/horror-monster/source/Monster_WalkingPr.fbx", "Assets/Prefabs/Characters/Demon_Monster.prefab", 2.10f, matMonster);

        // Helper: Instantiate Prefab or Fallback
        System.Func<GameObject, string, Vector3, Vector3, GameObject> SpawnPrefab = (prefab, name, pos, euler) => {
            GameObject instance;
            if (prefab != null)
            {
                instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = name;
            }
            else
            {
                instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                instance.name = name + " (Fallback)";
            }
            instance.transform.position = pos;
            instance.transform.rotation = Quaternion.Euler(euler);
            return instance;
        };

        // Helper: Spawn Interactive Door into doorway
        System.Func<Vector3, Vector3, string, bool, string, GameObject> SpawnDoor = (pos, euler, name, isLocked, reqKey) => {
            GameObject doorObj = SpawnPrefab(prefabDoor, name, pos, euler);
            
            // Setup DoorInteractive
            DoorInteractive doorComp = doorObj.GetComponent<DoorInteractive>();
            if (doorComp == null) doorComp = doorObj.AddComponent<DoorInteractive>();
            doorComp.SetLocked(isLocked, reqKey);

            // Ensure BoxCollider for interaction & blocking
            BoxCollider col = doorObj.GetComponent<BoxCollider>();
            if (col == null) col = doorObj.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 1.07f, 0);
            col.size = new Vector3(1.1f, 2.15f, 0.25f);

            return doorObj;
        };

        // =================================================================
        // ARCHITECTURE: 4 ENCLOSED ROOMS CONNECTED BY CORRIDOR WITH DOORS
        // Total Area: 34m x 34m, Wall Height: 3.5m
        // =================================================================
        float H = 3.5f;
        float T = 0.35f;

        // Floor & Ceiling
        CreateBox("Mansion_Ground_Floor", new Vector3(0, -0.1f, 0), new Vector3(34, 0.2f, 34), matFloor);
        CreateBox("Mansion_Ceiling_Roof", new Vector3(0, H + 0.1f, 0), new Vector3(34, 0.2f, 34), matWall);

        // Outer 4 Boundary Walls
        CreateBox("Wall_Outer_North", new Vector3(0, H / 2, 17), new Vector3(34, H, T), matWall);
        CreateBox("Wall_Outer_South", new Vector3(0, H / 2, -17), new Vector3(34, H, T), matWall);
        CreateBox("Wall_Outer_East", new Vector3(17, H / 2, 0), new Vector3(T, H, 34), matWall);
        CreateBox("Wall_Outer_West", new Vector3(-17, H / 2, 0), new Vector3(T, H, 34), matWall);

        // -------------------------------------------------------------
        // WEST ROOMS (X: -17 to -2.5): Divided into Bedroom (North) & Library (South)
        // -------------------------------------------------------------
        // West corridor wall (X = -2.5) with doorways at Z: 8.5 (Bedroom) and Z: -8.5 (Library)
        CreateBox("Wall_West_Corr_North", new Vector3(-2.5f, H / 2, 14.25f), new Vector3(T, H, 5.5f), matWall); // Z: 11.5 to 17
        CreateBox("Lintel_Bedroom_Door", new Vector3(-2.5f, 2.85f, 8.5f), new Vector3(T, 1.3f, 1.3f), matWall); // Lintel over door
        CreateBox("Wall_West_Corr_Mid", new Vector3(-2.5f, H / 2, 0), new Vector3(T, H, 14.5f), matWall); // Z: -7.25 to 7.25
        CreateBox("Lintel_Library_Door", new Vector3(-2.5f, 2.85f, -8.5f), new Vector3(T, 1.3f, 1.3f), matWall); // Lintel over door
        CreateBox("Wall_West_Corr_South", new Vector3(-2.5f, H / 2, -14.25f), new Vector3(T, H, 5.5f), matWall); // Z: -17 to -11.5

        // Horizontal Wall dividing Bedroom and Library (Z = 0, X: -17 to -2.5)
        CreateBox("Wall_Divider_West_Rooms", new Vector3(-9.75f, H / 2, 0), new Vector3(14.5f, H, T), matWall);

        // Doors for West Rooms
        SpawnDoor(new Vector3(-2.5f, 0, 8.5f), new Vector3(0, 90, 0), "Door_Emily_Bedroom", false, "");
        SpawnDoor(new Vector3(-2.5f, 0, -8.5f), new Vector3(0, 90, 0), "Door_Locked_Library", true, "key_library");

        // -------------------------------------------------------------
        // EAST ROOMS (X: 2.5 to 17): Divided into Dining (North) & Storage (South)
        // -------------------------------------------------------------
        // East corridor wall (X = +2.5) with doorways at Z: 8.5 (Dining) and Z: -8.5 (Storage)
        CreateBox("Wall_East_Corr_North", new Vector3(2.5f, H / 2, 14.25f), new Vector3(T, H, 5.5f), matWall); // Z: 11.5 to 17
        CreateBox("Lintel_Dining_Door", new Vector3(2.5f, 2.85f, 8.5f), new Vector3(T, 1.3f, 1.3f), matWall); // Lintel over door
        CreateBox("Wall_East_Corr_Mid", new Vector3(2.5f, H / 2, 0), new Vector3(T, H, 14.5f), matWall); // Z: -7.25 to 7.25
        CreateBox("Lintel_Storage_Door", new Vector3(2.5f, 2.85f, -8.5f), new Vector3(T, 1.3f, 1.3f), matWall); // Lintel over door
        CreateBox("Wall_East_Corr_South", new Vector3(2.5f, H / 2, -14.25f), new Vector3(T, H, 5.5f), matWall); // Z: -17 to -11.5

        // Horizontal Wall dividing Dining and Storage (Z = 0, X: 2.5 to 17)
        CreateBox("Wall_Divider_East_Rooms", new Vector3(9.75f, H / 2, 0), new Vector3(14.5f, H, T), matWall);

        // Doors for East Rooms
        SpawnDoor(new Vector3(2.5f, 0, 8.5f), new Vector3(0, -90, 0), "Door_Dining_Room", false, "");
        SpawnDoor(new Vector3(2.5f, 0, -8.5f), new Vector3(0, -90, 0), "Door_Storage_Room", false, "");

        // =================================================================
        // FURNISHING ROOMS WITH ACCURATELY SCALED PREFABS
        // =================================================================

        // --- ROOM 1: EMILY'S BEDROOM (North-West) ---
        SpawnPrefab(prefabBed, "Prop_Victorian_Bed", new Vector3(-12.5f, 0, 13f), new Vector3(0, 90, 0));
        SpawnPrefab(prefabDesk, "Prop_Bedroom_Desk", new Vector3(-7f, 0, 14.5f), new Vector3(0, 180, 0));

        var safeBed = SpawnPrefab(prefabSafe, "Safe_Bedroom", new Vector3(-7f, 0.78f, 14.5f), new Vector3(0, 180, 0));
        var lockBed = safeBed.GetComponent<Lockable>();
        if (lockBed == null) lockBed = safeBed.AddComponent<Lockable>();
        var soBed = new SerializedObject(lockBed);
        soBed.FindProperty("lockType").enumValueIndex = (int)LockType.Code;
        soBed.FindProperty("lockId").stringValue = "safe_bedroom";
        soBed.FindProperty("correctCode").stringValue = "1894";
        soBed.FindProperty("interactionPrompt").stringValue = "Locked Safe (Enter Code: 1894)";
        soBed.ApplyModifiedProperties();

        // Clue Note A4 size on table
        var clueNote = CreateBox("Clue_Note_Emily", new Vector3(-7.7f, 0.79f, 14.5f), new Vector3(0.28f, 0.01f, 0.20f), matPaper);
        var clueComp = clueNote.AddComponent<CodeClue>();
        var soClue = new SerializedObject(clueComp);
        soClue.FindProperty("clueId").stringValue = "clue_emily_diary";
        soClue.FindProperty("clueText").stringValue = "Read torn diary: The year Father bought the mirror was 1894...";
        soClue.FindProperty("interactionPrompt").stringValue = "Read torn diary (E / Click)";
        soClue.ApplyModifiedProperties();

        CreatePointLight("Light_Candle_Bedroom", new Vector3(-9.5f, 2.2f, 9.5f), 16f, 12f, new Color(1f, 0.8f, 0.45f));

        // --- ROOM 2: LIBRARY / STUDY (South-West) ---
        SpawnPrefab(prefabBookshelf, "Prop_Bookshelf_West_1", new Vector3(-15.5f, 0, -6f), new Vector3(0, 90, 0));
        SpawnPrefab(prefabBookshelf, "Prop_Bookshelf_West_2", new Vector3(-15.5f, 0, -11f), new Vector3(0, 90, 0));
        SpawnPrefab(prefabBookshelf, "Prop_Bookshelf_South", new Vector3(-9f, 0, -15.5f), Vector3.zero);
        SpawnPrefab(prefabDesk, "Prop_Library_StudyDesk", new Vector3(-9f, 0, -8.5f), Vector3.zero);

        var safeLib = SpawnPrefab(prefabSafe, "Safe_Library", new Vector3(-9f, 0.78f, -8.5f), Vector3.zero);
        var lockLib = safeLib.GetComponent<Lockable>();
        if (lockLib == null) lockLib = safeLib.AddComponent<Lockable>();
        var soLib = new SerializedObject(lockLib);
        soLib.FindProperty("lockType").enumValueIndex = (int)LockType.Code;
        soLib.FindProperty("lockId").stringValue = "safe_library";
        soLib.FindProperty("correctCode").stringValue = "5731";
        soLib.FindProperty("interactionPrompt").stringValue = "Occult Safe (Code: 5731)";
        soLib.ApplyModifiedProperties();

        CreatePointLight("Light_Candle_Library", new Vector3(-9.5f, 2.2f, -9.5f), 16f, 12f, new Color(1f, 0.75f, 0.4f));

        // --- ROOM 3: DINING ROOM (North-East) ---
        SpawnPrefab(prefabDining, "Prop_Dining_Table_Set", new Vector3(9.5f, 0, 8.5f), Vector3.zero);

        // Gold Key on Dining Table (0.08m)
        var keyLib = SpawnPrefab(prefabKey, "Key_Library_Gold", new Vector3(9.5f, 0.80f, 8.5f), new Vector3(90, 0, 0));
        var keyComp = keyLib.GetComponent<KeyItem>();
        if (keyComp == null) keyComp = keyLib.AddComponent<KeyItem>();
        var soKey = new SerializedObject(keyComp);
        soKey.FindProperty("keyId").stringValue = "key_library";
        soKey.FindProperty("interactionPrompt").stringValue = "Pick up Ornate Gold Key";
        soKey.ApplyModifiedProperties();

        CreatePointLight("Light_Candle_Dining", new Vector3(9.5f, 2.2f, 8.5f), 18f, 14f, new Color(1f, 0.8f, 0.45f));

        // --- ROOM 4: STORAGE ROOM (South-East) ---
        SpawnPrefab(prefabBarrel, "Prop_Storage_Barrel_1", new Vector3(13.5f, 0, -13.5f), Vector3.zero);
        SpawnPrefab(prefabBarrel, "Prop_Storage_Barrel_2", new Vector3(14.5f, 0, -11.5f), Vector3.zero);
        SpawnPrefab(prefabBarrel, "Prop_Storage_Barrel_3", new Vector3(7.5f, 0, -14.5f), Vector3.zero);

        // Storage Safe with Ritual Dagger
        var safeStorage = SpawnPrefab(prefabSafe, "Safe_Storage_Dagger", new Vector3(10f, 0, -8.5f), Vector3.zero);
        var lockStorage = safeStorage.GetComponent<Lockable>();
        if (lockStorage == null) lockStorage = safeStorage.AddComponent<Lockable>();
        var soStor = new SerializedObject(lockStorage);
        soStor.FindProperty("lockType").enumValueIndex = (int)LockType.Code;
        soStor.FindProperty("lockId").stringValue = "chest_storage";
        soStor.FindProperty("correctCode").stringValue = "5731";
        soStor.FindProperty("interactionPrompt").stringValue = "Locked Metal Safe (Code: 5731)";
        soStor.ApplyModifiedProperties();

        // Ritual Dagger (0.30m)
        var daggerItem = SpawnPrefab(prefabDagger, "KeyItem_RitualDagger", new Vector3(10f, 0.60f, -8.5f), new Vector3(0, 45, 90));
        var daggerComp = daggerItem.GetComponent<KeyItem>();
        if (daggerComp == null) daggerComp = daggerItem.AddComponent<KeyItem>();
        var soDagger = new SerializedObject(daggerComp);
        soDagger.FindProperty("keyId").stringValue = "key_ritual_dagger";
        soDagger.FindProperty("interactionPrompt").stringValue = "Pick up Royal Ritual Dagger";
        soDagger.ApplyModifiedProperties();

        CreatePointLight("Light_Candle_Storage", new Vector3(9.5f, 2.2f, -9.5f), 16f, 11f, new Color(1f, 0.7f, 0.35f));

        // --- ROOM 5: RITUAL SANCTUARY & GRAND MIRROR (North Corridor End) ---
        var altar = SpawnPrefab(prefabAltar, "Prop_Ritual_Altar", new Vector3(0, 0, 12f), Vector3.zero);
        var lockAltar = altar.GetComponent<Lockable>();
        if (lockAltar == null) lockAltar = altar.AddComponent<Lockable>();
        var soAltar = new SerializedObject(lockAltar);
        soAltar.FindProperty("lockType").enumValueIndex = (int)LockType.Key;
        soAltar.FindProperty("lockId").stringValue = "altar_ritual";
        soAltar.FindProperty("requiredKeyId").stringValue = "key_ritual_dagger";
        soAltar.FindProperty("interactionPrompt").stringValue = "Perform Blood Rite with Ritual Dagger";
        soAltar.ApplyModifiedProperties();

        CreatePointLight("Light_Ritual_Altar_Red", new Vector3(0, 1.8f, 12f), 18f, 16f, new Color(1f, 0.22f, 0.1f));

        var exitMirror = SpawnPrefab(prefabMirror, "GrandGatewayMirror_Exit", new Vector3(0, 0, 16.3f), new Vector3(0, 180, 0));
        var lockMirror = exitMirror.GetComponent<Lockable>();
        if (lockMirror == null) lockMirror = exitMirror.AddComponent<Lockable>();
        var soMirror = new SerializedObject(lockMirror);
        soMirror.FindProperty("lockType").enumValueIndex = (int)LockType.Key;
        soMirror.FindProperty("lockId").stringValue = "exit_gateway_mirror";
        soMirror.FindProperty("requiredKeyId").stringValue = "key_mirror_shard";
        soMirror.FindProperty("interactionPrompt").stringValue = "Grand Gateway Mirror (Break the mirror seal to escape!)";
        soMirror.ApplyModifiedProperties();

        // Corridor Lights
        CreatePointLight("Light_Torch_Corridor_N", new Vector3(0, 2.5f, 5f), 16f, 12f, new Color(1f, 0.75f, 0.4f));
        CreatePointLight("Light_Torch_Corridor_S", new Vector3(0, 2.5f, -6f), 16f, 12f, new Color(1f, 0.75f, 0.4f));

        // =================================================================
        // WAYPOINTS FOR PATROL
        // =================================================================
        GameObject waypointsParent = new GameObject("--- AI_WAYPOINTS ---");
        Vector3[] wpPositions = new Vector3[] {
            new Vector3(0, 0.1f, -12f),   // South Hall
            new Vector3(0, 0.1f, -4f),    // Mid-South Hall
            new Vector3(9.5f, 0.1f, 8.5f),// Dining Room
            new Vector3(0, 0.1f, 4f),     // Mid-North Hall
            new Vector3(0, 0.1f, 12f),    // Ritual Chamber
            new Vector3(-7f, 0.1f, 9f),   // Bedroom Hallway
        };

        Transform[] waypoints = new Transform[wpPositions.Length];
        for (int i = 0; i < wpPositions.Length; i++)
        {
            GameObject wp = new GameObject($"Waypoint_{i + 1}");
            wp.transform.position = wpPositions[i];
            wp.transform.SetParent(waypointsParent.transform, true);
            waypoints[i] = wp.transform;
        }

        // =================================================================
        // 3D HORROR MONSTER SETUP (Walking Demon)
        // =================================================================
        GameObject monster = SpawnPrefab(prefabMonster, "Mirror_Demon_Monster", new Vector3(0, 0.05f, -4f), Vector3.zero);
        
        var capCol = monster.GetComponent<CapsuleCollider>();
        if (capCol == null) capCol = monster.AddComponent<CapsuleCollider>();
        capCol.center = new Vector3(0, 1.05f, 0);
        capCol.radius = 0.5f;
        capCol.height = 2.1f;

        var nma = monster.GetComponent<NavMeshAgent>();
        if (nma == null) nma = monster.AddComponent<NavMeshAgent>();
        nma.speed = 2.5f;
        nma.stoppingDistance = 1.2f;
        nma.radius = 0.45f;
        nma.height = 2.1f;

        var ai = monster.GetComponent<AIController>();
        if (ai == null) ai = monster.AddComponent<AIController>();
        ai.detectionRadius = 14f;
        ai.attackRadius = 1.8f;
        ai.patrolSpeed = 2.2f;
        ai.chaseSpeed = 4.2f;
        ai.patrolPoints = waypoints;

        CreatePointLight("Light_Monster_Aura", new Vector3(0, 1.2f, -4f), 7f, 6f, new Color(1f, 0.1f, 0.1f));

        // =================================================================
        // PLAYER EMILY (FPS)
        // =================================================================
        GameObject player = new GameObject("Player_Emily");
        player.tag = "Player";
        player.transform.position = new Vector3(-6f, 0.1f, 8.5f); // Inside Emily's bedroom near door
        player.transform.rotation = Quaternion.Euler(0, 90f, 0);

        var charCtrl = player.AddComponent<CharacterController>();
        charCtrl.height = 1.8f;
        charCtrl.radius = 0.35f;
        charCtrl.center = new Vector3(0f, 0.9f, 0f);
        player.AddComponent<PlayerController>();

        ai.playerTarget = player.transform;

        // Camera
        GameObject camGo = new GameObject("FirstPersonCamera");
        camGo.tag = "MainCamera";
        camGo.transform.SetParent(player.transform, false);
        camGo.transform.localPosition = new Vector3(0f, 1.6f, 0f);

        var cam = camGo.AddComponent<Camera>();
        cam.backgroundColor = new Color(0.04f, 0.03f, 0.05f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 100f;
        cam.fieldOfView = 70f;
        camGo.AddComponent<AudioListener>();
        camGo.AddComponent<UniversalAdditionalCameraData>();

        var playerCam = camGo.AddComponent<PlayerCamera>();
        var fieldBody = typeof(PlayerCamera).GetField("playerBody", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fieldBody != null) fieldBody.SetValue(playerCam, player.transform);

        // Flashlight
        GameObject flashGo = new GameObject("Player_Lantern_Flashlight");
        flashGo.transform.SetParent(camGo.transform, false);
        flashGo.transform.localPosition = new Vector3(0, 0, 0.1f);
        Light flashLight = flashGo.AddComponent<Light>();
        flashLight.type = LightType.Spot;
        flashLight.spotAngle = 70f;
        flashLight.innerSpotAngle = 40f;
        flashLight.range = 25f;
        flashLight.intensity = 15f;
        flashLight.color = new Color(1f, 0.95f, 0.85f);
        flashLight.shadows = LightShadows.Soft;

        EditorSceneManager.MarkSceneDirty(currentScene);
        EditorSceneManager.SaveScene(currentScene);

        // Print Model Calibration Report
        PrintCalibrationReport();
    }

    private static void EnsureDirectories()
    {
        string[] dirs = new string[] {
            "Assets/Models/Props",
            "Assets/Models/Furniture",
            "Assets/Models/Environment",
            "Assets/Models/Characters",
            "Assets/Prefabs/Props",
            "Assets/Prefabs/Furniture",
            "Assets/Prefabs/Characters"
        };
        foreach (var dir in dirs)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        AssetDatabase.Refresh();
    }

    private static void PrintCalibrationReport()
    {
        Debug.Log("<color=green><b>=======================================================</b></color>");
        Debug.Log("<color=green><b>[MirrorHouse] HOÀN TẤT CHUẨN HÓA MODEL 3D & XÂY DỰNG SCENE WORLD 2:</b></color>");
        Debug.Log("<b>1. Chìa khóa (Key_Gold):</b> Chiều dài thực tế 0.08m (8cm) - Đặt trên bàn ăn.");
        Debug.Log("<b>2. Dao tế lễ (Ritual_Dagger):</b> Chiều dài thực tế 0.30m (30cm) - Đặt trong phòng kho.");
        Debug.Log("<b>3. Giấy manh mối (Clue_Note):</b> Kích thước chuẩn A4 (0.28m x 0.20m) - Đặt trên bàn ngủ.");
        Debug.Log("<b>4. Két sắt (Safe_Box):</b> Chiều cao chuẩn 0.55m - Đặt trên bàn ngủ và bàn thư viện.");
        Debug.Log("<b>5. Thùng gỗ (Storage_Barrel):</b> Chiều cao chuẩn 0.80m - Đặt tại phòng kho.");
        Debug.Log("<b>6. Cửa gỗ (Victorian_Door):</b> Cao 2.15m, Rộng 1.05m - Gắn DoorInteractive mở/đóng bằng Click/E.");
        Debug.Log("<b>7. Giường Victorian (Victorian_Bed):</b> Dài 2.1m, Rộng 1.6m, Cao 1.25m - Phòng ngủ Emily.");
        Debug.Log("<b>8. Bàn làm việc (Vintage_Desk):</b> Cao 0.78m, Dài 1.4m - Phòng ngủ & Thư viện.");
        Debug.Log("<b>9. Tủ sách (Victorian_Bookshelf):</b> Cao 2.05m, Rộng 1.2m - Áp tường Thư viện.");
        Debug.Log("<b>10. Bàn ăn (Dining_Table_Set):</b> Cao 0.78m, Dài 2.2m - Trung tâm Phòng ăn.");
        Debug.Log("<b>11. Bàn thờ đá (Ritual_Altar):</b> Cao 0.85m, Dài 1.8m - Điện thờ Bắc.");
        Debug.Log("<b>12. Gương lớn (Grand_Mirror):</b> Cao 2.20m, Rộng 1.2m - Cổng thoát hiểm Bắc.");
        Debug.Log("<b>13. Quái vật Demon (Demon_Monster):</b> Cao 2.10m - Đầy đủ SkinnedMesh, NavMesh, AI Waypoints tuần tra.");
        Debug.Log("<color=green><b>=======================================================</b></color>");
    }
}
#endif