// SceneFurnitureAdjuster.cs
// Fix furniture positioning and scale issues in Level_Main scene
// Use: MirrorHouse → Fix Scene Furniture

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SceneFurnitureAdjuster : EditorWindow
{
    // Furniture position/scale reference data
    private static readonly FurnitureData[] furnitureList = new FurnitureData[]
    {
        // Bedroom furniture
        new FurnitureData("Prop_Victorian_Bed", new Vector3(-12.5f, 0, 13f), 1.0f),
        new FurnitureData("Prop_Bedroom_Desk", new Vector3(-7f, 0, 14.5f), 1.0f),
        new FurnitureData("Safe_Bedroom", new Vector3(-7f, 0.78f, 14.5f), 1.0f), // On desk
        
        // Library furniture
        new FurnitureData("Prop_Bookshelf_West_1", new Vector3(-15.5f, 0, -6f), 1.0f),
        new FurnitureData("Prop_Bookshelf_West_2", new Vector3(-15.5f, 0, -11f), 1.0f),
        new FurnitureData("Prop_Bookshelf_South", new Vector3(-9f, 0, -15.5f), 1.0f),
        new FurnitureData("Prop_Library_StudyDesk", new Vector3(-9f, 0, -8.5f), 1.0f),
        new FurnitureData("Safe_Library", new Vector3(-9f, 0.78f, -8.5f), 1.0f), // On desk
        
        // Storage room furniture
        new FurnitureData("Prop_Storage_Barrel_1", new Vector3(13.5f, 0, -13.5f), 1.0f),
        new FurnitureData("Prop_Storage_Barrel_2", new Vector3(14.5f, 0, -11.5f), 1.0f),
        new FurnitureData("Prop_Storage_Barrel_3", new Vector3(7.5f, 0, -14.5f), 1.0f),
        new FurnitureData("Safe_Storage_Dagger", new Vector3(10f, 0, -8.5f), 1.0f),
        
        // Ritual room furniture
        new FurnitureData("Prop_Ritual_Altar", new Vector3(0, 0, 12f), 1.0f),
        
        // Exit mirror
        new FurnitureData("GrandGatewayMirror_Exit", new Vector3(0, 0, 16.3f), 1.0f),
    };

    [MenuItem("MirrorHouse/Fix Scene Furniture (Position & Scale)")]
    public static void FixFurniture()
    {
        // Ensure Level_Main is loaded
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main")
        {
            bool openScene = EditorUtility.DisplayDialog(
                "Wrong Scene Active",
                $"Current scene: {activeScene.name}\n\nThis script needs Level_Main.unity.\n\nOpen Level_Main.unity now?",
                "Yes, Open Level_Main",
                "Cancel"
            );
            
            if (openScene)
            {
                string scenePath = "Assets/Scenes/Level_Main.unity";
                if (!System.IO.File.Exists(scenePath))
                {
                    EditorUtility.DisplayDialog("Error", "Level_Main.unity not found!", "OK");
                    return;
                }
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                Debug.Log("[Furniture Fix] Opened Level_Main.unity");
            }
            else
            {
                Debug.LogWarning("[Furniture Fix] Cancelled - Level_Main.unity must be active");
                return;
            }
        }

        int fixedCount = 0;
        int notFoundCount = 0;

        foreach (var data in furnitureList)
        {
            GameObject obj = GameObject.Find(data.name);
            
            if (obj == null)
            {
                Debug.LogWarning($"[Furniture Fix] Not found: {data.name}");
                notFoundCount++;
                continue;
            }

            // Fix position
            Vector3 oldPos = obj.transform.position;
            obj.transform.position = data.position;
            
            // Optional: Fix scale if needed (currently keeps existing scale)
            // Uncomment if you want to reset scale:
            // obj.transform.localScale = Vector3.one * data.scale;
            
            Debug.Log($"[Furniture Fix] {data.name}: Position {oldPos} → {data.position}");
            fixedCount++;
        }

        Debug.Log($"[Furniture Fix] Complete! Fixed: {fixedCount}, Not found: {notFoundCount}");

        // Mark scene dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );

        EditorUtility.DisplayDialog(
            "Furniture Fix Complete",
            $"Fixed {fixedCount} furniture objects.\n{notFoundCount} objects not found (might be OK if scene was modified).\n\nSave scene to keep changes.",
            "OK"
        );
    }

    [MenuItem("MirrorHouse/Audit Scene Furniture (Report Only)")]
    public static void AuditFurniture()
    {
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main")
        {
            EditorUtility.DisplayDialog("Wrong Scene", $"Current scene: {activeScene.name}\n\nNeed Level_Main.unity", "OK");
            return;
        }

        Debug.Log("=== FURNITURE AUDIT REPORT ===");
        
        foreach (var data in furnitureList)
        {
            GameObject obj = GameObject.Find(data.name);
            
            if (obj == null)
            {
                Debug.LogWarning($"❌ {data.name} - NOT FOUND");
                continue;
            }

            Vector3 pos = obj.transform.position;
            Vector3 scale = obj.transform.localScale;
            
            bool positionOK = Vector3.Distance(pos, data.position) < 0.1f;
            
            string status = positionOK ? "✅" : "⚠️";
            Debug.Log($"{status} {data.name}");
            Debug.Log($"   Position: Expected {data.position}, Actual {pos}");
            Debug.Log($"   Scale: {scale}");
            
            if (!positionOK)
            {
                float yDiff = pos.y - data.position.y;
                if (Mathf.Abs(yDiff) > 0.1f)
                {
                    Debug.LogWarning($"   ⚠️ Y position off by {yDiff:F2}m (floating: {yDiff > 0}, sunken: {yDiff < 0})");
                }
            }
        }
        
        Debug.Log("=== END AUDIT REPORT ===");
    }

    // Helper struct
    [System.Serializable]
    private struct FurnitureData
    {
        public string name;
        public Vector3 position;
        public float scale;

        public FurnitureData(string name, Vector3 position, float scale)
        {
            this.name = name;
            this.position = position;
            this.scale = scale;
        }
    }
}
#endif
