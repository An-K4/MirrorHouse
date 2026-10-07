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
        int addedCount = 0;
        int skippedCount = 0;

        foreach (string wallName in wallNames)
        {
            GameObject wall = GameObject.Find(wallName);
            
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
        EditorUtility.SetDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().GetRootGameObjects()[0]);
    }
}
#endif
