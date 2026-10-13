#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Editor entry point to preview or rebuild the Mirror House horror HUD.</summary>
public static class MirrorHouseHorrorHUDBuilder
{
    [MenuItem("MirrorHouse/Apply Horror Mirror HUD to Level_Main", false, 3)]
    public static void ApplyToLevelMain()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Level_Main")
        {
            scene = EditorSceneManager.OpenScene("Assets/Scenes/Level_Main.unity");
        }

        MirrorHouseHorrorHUD hud = MirrorHouseHorrorHUD.EnsureExists();
        if (hud == null)
        {
            EditorUtility.DisplayDialog("Mirror House HUD", "No Canvas was found in Level_Main. Run 'Setup UI for Level_Main' first.", "OK");
            return;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = hud.gameObject;
        Debug.Log("[MirrorHouseHorrorHUDBuilder] Horror HUD applied to Level_Main.");
    }
}
#endif