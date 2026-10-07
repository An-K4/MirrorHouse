using UnityEngine;

public class RoleC_PuzzleSetup
{
    public static void SetupPuzzles()
    {
        Debug.Log("Setting up puzzle objects...");

        GameObject puzzles = GameObject.Find("Puzzles");
        if (puzzles == null)
        {
            puzzles = new GameObject("Puzzles");
        }

        // 4 Puzzle Locks
        CreatePuzzleObject(puzzles, "Lock_Door_MainExit", 1, "PuzzleLock", new Vector3(0, 1, -12));
        CreatePuzzleObject(puzzles, "Lock_Door_Kitchen", 1, "PuzzleLock", new Vector3(-8, 1, -9.5f));
        CreatePuzzleObject(puzzles, "Lock_Door_Bedroom", 1, "PuzzleLock", new Vector3(0, 1, 3));
        CreatePuzzleObject(puzzles, "Lock_Door_Bathroom", 1, "PuzzleLock", new Vector3(10, 1, -3));

        // 2 Puzzle Keys
        CreatePuzzleObject(puzzles, "Key_Brass_MainHall", 0.3f, "PuzzleKey", new Vector3(1, 1, -10));
        CreatePuzzleObject(puzzles, "Key_Silver_Kitchen", 0.3f, "PuzzleKey", new Vector3(-6, 1, -13));

        // 2 Puzzle Clues
        CreatePuzzleObject(puzzles, "Clue_AnniversaryPhoto", 0.8f, "PuzzleClue", new Vector3(-2, 2, 3));
        CreatePuzzleObject(puzzles, "Clue_KitchenRecipe", 0.8f, "PuzzleClue", new Vector3(-7, 2, -10));

        Debug.Log("✓ Puzzles setup complete!");
    }

    static void CreatePuzzleObject(GameObject parent, string name, float scale, string type, Vector3 pos)
    {
        GameObject puzzle = new GameObject(name);
        puzzle.transform.SetParent(parent.transform);
        puzzle.transform.position = pos;

        MeshFilter mf = puzzle.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        MeshRenderer mr = puzzle.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));

        if (type == "PuzzleLock")
        {
            mat.color = new Color(1f, 0f, 0f, 0.7f);
        }
        else if (type == "PuzzleKey")
        {
            mat.color = new Color(1f, 1f, 0f);
        }
        else if (type == "PuzzleClue")
        {
            mat.color = new Color(0f, 0f, 1f);
        }
        mr.material = mat;

        BoxCollider bc = puzzle.AddComponent<BoxCollider>();
        bc.size = new Vector3(scale, scale, scale);
        bc.isTrigger = true;

        puzzle.tag = type;
        puzzle.transform.localScale = new Vector3(scale, scale, scale);

        Debug.Log($"Created {type}: {name}");
    }
}