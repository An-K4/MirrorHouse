// FixNavMeshWalls.cs
// Unity 6 NavMesh fix: Add NavMeshModifier to walls
// Modern system (AI Navigation package 2.0+) thay thế "Navigation Static" flags

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;

public class FixNavMeshWalls : EditorWindow
{
    private static readonly string[] wallNames = new string[]
    {
        "Wall_Outer_North",
        "Wall_Outer_South",
        "Wall_Outer_East",
        "Wall_Outer_West",
        "Wall_East_Corr_North",
        "Wall_East_Corr_Mid",
        "Wall_East_Corr_South",
        "Wall_West_Corr_North",
        "Wall_West_Corr_Mid",
        "Wall_West_Corr_South",
        "Wall_Divider_East_Rooms",
        "Wall_Divider_West_Rooms"
    };

    [MenuItem("MirrorHouse/Fix NavMesh Walls (Unity 6)")]
    public static void FixWalls()
    {
        // Ensure Level_Main.unity is loaded
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name != "Level_Main")
        {
            string scenePath = "Assets/Scenes/Level_Main.unity";
            if (!System.IO.File.Exists(scenePath))
            {
                EditorUtility.DisplayDialog("Error", "Level_Main.unity not found at: " + scenePath, "OK");
                return;
            }
            
            bool openScene = EditorUtility.DisplayDialog(
                "Wrong Scene Active",
                $"Current scene: {activeScene.name}\n\nThis script needs Level_Main.unity to be open.\n\nOpen Level_Main.unity now?",
                "Yes, Open Level_Main",
                "Cancel"
            );
            
            if (openScene)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                Debug.Log("[NavMesh Fix] Opened Level_Main.unity");
            }
            else
            {
                Debug.LogWarning("[NavMesh Fix] Cancelled - Level_Main.unity must be active scene");
                return;
            }
        }

        int addedCount = 0;
        int skippedCount = 0;

        // Get all GameObjects in scene (including children)
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            .GetRootGameObjects();
        
        foreach (string wallName in wallNames)
        {
            GameObject wall = FindGameObjectByName(allObjects, wallName);
            
            if (wall == null)
            {
                Debug.LogWarning($"[NavMesh Fix] Wall not found: {wallName}");
                continue;
            }

            // Check if already has NavMeshModifier
            NavMeshModifier modifier = wall.GetComponent<NavMeshModifier>();
            
            if (modifier == null)
            {
                // Add NavMeshModifier component
                modifier = wall.AddComponent<NavMeshModifier>();
                
                // Default settings: walls are obstacles (Remove Object mode)
                // Unity will NOT create NavMesh surface inside these objects
                modifier.overrideArea = false;
                modifier.ignoreFromBuild = false;
                
                Debug.Log($"[NavMesh Fix] Added NavMeshModifier to: {wallName}");
                addedCount++;
            }
            else
            {
                Debug.Log($"[NavMesh Fix] {wallName} already has NavMeshModifier (skipped)");
                skippedCount++;
            }
        }

        Debug.Log($"[NavMesh Fix] Complete! Added: {addedCount}, Skipped: {skippedCount}");

        // Find NavMeshSurface and prompt to rebake
        NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
        
        if (surface != null)
        {
            bool rebake = EditorUtility.DisplayDialog(
                "Rebake NavMesh?",
                $"Added NavMeshModifier to {addedCount} walls.\n\n" +
                "Do you want to rebake the NavMesh now?\n\n" +
                "(Recommended: Yes - this will update NavMesh to respect wall obstacles)",
                "Yes, Rebake Now",
                "No, I'll Rebake Manually"
            );

            if (rebake)
            {
                Debug.Log("[NavMesh Fix] Rebaking NavMesh...");
                surface.BuildNavMesh();
                Debug.Log("[NavMesh Fix] NavMesh rebaked successfully!");
            }
            else
            {
                Debug.Log("[NavMesh Fix] Remember to rebake NavMesh: Select NavMeshSurface → Inspector → Bake button");
            }
        }
        else
        {
            Debug.LogWarning("[NavMesh Fix] No NavMeshSurface found in scene. Add one and bake manually.");
        }

        // Mark scene as dirty to save changes
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }

    // Helper: Recursively search for GameObject by name in hierarchy
    private static GameObject FindGameObjectByName(GameObject[] rootObjects, string name)
    {
        foreach (GameObject root in rootObjects)
        {
            if (root.name == name)
                return root;

            GameObject found = FindInChildren(root.transform, name);
            if (found != null)
                return found;
        }
        return null;
    }

    private static GameObject FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject found = FindInChildren(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
}
#endif
