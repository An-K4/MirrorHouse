using UnityEditor;
using UnityEngine;

public class SetupLevelC
{
    [MenuItem("Tools/Level_C/Setup Scene Structure")]
    public static void SetupSceneStructure()
    {
        Debug.Log("Setting up Level C scene structure...");

        CreateOrGetGameObject("Geometry");
        CreateOrGetGameObject("Geometry/Rooms");
        CreateOrGetGameObject("Geometry/Walls");
        CreateOrGetGameObject("Furniture");
        CreateOrGetGameObject("Puzzles");
        CreateOrGetGameObject("AI");
        CreateOrGetGameObject("Player");
        CreateOrGetGameObject("Environment");

        Debug.Log("✓ Setup complete!");
    }

    static GameObject CreateOrGetGameObject(string path)
    {
        string[] parts = path.Split('/');
        GameObject current = null;

        foreach (string part in parts)
        {
            GameObject next = null;
            if (current == null)
            {
                next = GameObject.Find(part);
                if (next == null)
                {
                    next = new GameObject(part);
                }
            }
            else
            {
                Transform child = current.transform.Find(part);
                if (child == null)
                {
                    next = new GameObject(part);
                    next.transform.SetParent(current.transform);
                }
                else
                {
                    next = child.gameObject;
                }
            }
            current = next;
        }
        return current;
    }
}