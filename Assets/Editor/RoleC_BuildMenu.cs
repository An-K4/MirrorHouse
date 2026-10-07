using UnityEditor;
using UnityEngine;

public class RoleC_BuildMenu
{
    [MenuItem("Tools/Role_C/Build - STEP 1 Setup Structure")]
    public static void MenuSetupStructure()
    {
        SetupLevelC.SetupSceneStructure();
    }

    [MenuItem("Tools/Role_C/Build - STEP 2 Rooms & Furniture")]
    public static void MenuBuildRoomsAndFurniture()
    {
        RoleC_LevelBuilder.BuildCompleteLevelC();
    }

    [MenuItem("Tools/Role_C/Build - STEP 3 Puzzles")]
    public static void MenuSetupPuzzles()
    {
        RoleC_PuzzleSetup.SetupPuzzles();
    }

    [MenuItem("Tools/Role_C/Build - STEP 4 Waypoints")]
    public static void MenuSetupWaypoints()
    {
        RoleC_AIWaypoints.SetupWaypoints();
    }

    [MenuItem("Tools/Role_C/BUILD ALL (Complete Level)")]
    public static void MenuBuildAll()
    {
        Debug.Log("========== BUILDING COMPLETE LEVEL_C ==========");
        SetupLevelC.SetupSceneStructure();
        RoleC_LevelBuilder.BuildCompleteLevelC();
        RoleC_PuzzleSetup.SetupPuzzles();
        RoleC_AIWaypoints.SetupWaypoints();
        Debug.Log("========== BUILD COMPLETE! Bake NavMesh next ==========");
        EditorUtility.DisplayDialog("Role C Build", "Level build complete!\n\nNext: Bake NavMesh\nWindow > AI > Navigation > Bake", "OK");
    }
}