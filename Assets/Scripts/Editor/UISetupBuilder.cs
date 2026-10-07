#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// UI SETUP BUILDER - Add mobile UI to Level_Main scene
/// 
/// USAGE: MirrorHouse → Setup UI for Level_Main
/// 
/// This adds:
/// - Canvas (root UI)
/// - Fixed Joystick (from Joystick Pack)
/// - MobileInteractButton (mobile E key)
/// - CodeInputPanel (for code locks)
/// </summary>
public class UISetupBuilder : EditorWindow
{
    [MenuItem("MirrorHouse/Setup UI for Level_Main", false, 2)]
    public static void SetupUI()
    {
        // Open Level_Main scene
        var currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.name != "Level_Main")
        {
            string scenePath = "Assets/Scenes/Level_Main.unity";
            if (File.Exists(scenePath))
            {
                currentScene = EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Level_Main.unity not found!", "OK");
                return;
            }
        }

        if (!EditorUtility.DisplayDialog(
            "Setup UI for Level_Main",
            "This will add Canvas, Joystick, MobileInteractButton, and CodeInputPanel to Level_Main scene. Continue?",
            "Setup UI",
            "Cancel"))
        {
            return;
        }

        // Check if Canvas already exists
        Canvas existingCanvas = GameObject.FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            if (!EditorUtility.DisplayDialog(
                "Canvas Already Exists",
                "A Canvas already exists in the scene. Delete it and recreate?",
                "Recreate",
                "Cancel"))
            {
                return;
            }
            Undo.DestroyObjectImmediate(existingCanvas.gameObject);
        }

        // Create UI
        CreateCanvas();
        CreateJoystick();
        CreateMobileInteractButton();
        CreateCodeInputPanel();

        // Save scene
        EditorSceneManager.SaveScene(currentScene);
        
        Debug.Log("[UISetupBuilder] ✅ UI setup complete for Level_Main!");
        EditorUtility.DisplayDialog("Success", "UI setup complete!\n\n- Canvas\n- Joystick\n- MobileInteractButton\n- CodeInputPanel", "OK");
    }

    static void CreateCanvas()
    {
        // Create Canvas GameObject
        GameObject canvasObj = new GameObject("Canvas");
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");

        // Add Canvas component
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Add CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Add GraphicRaycaster
        canvasObj.AddComponent<GraphicRaycaster>();

        Debug.Log("[UISetupBuilder] ✅ Created Canvas");
    }

    static void CreateJoystick()
    {
        // Load Fixed Joystick prefab
        string prefabPath = "Assets/Joystick Pack/Prefabs/Fixed Joystick.prefab";
        GameObject joystickPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (joystickPrefab == null)
        {
            Debug.LogError($"[UISetupBuilder] ❌ Fixed Joystick prefab not found at {prefabPath}");
            return;
        }

        // Instantiate joystick
        GameObject joystick = (GameObject)PrefabUtility.InstantiatePrefab(joystickPrefab);
        Undo.RegisterCreatedObjectUndo(joystick, "Create Joystick");

        // Parent to Canvas
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            joystick.transform.SetParent(canvas.transform, false);
        }

        // Position joystick at bottom-left
        RectTransform rect = joystick.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(100, 100);
        }

        Debug.Log("[UISetupBuilder] ✅ Created Fixed Joystick");
    }

    static void CreateMobileInteractButton()
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[UISetupBuilder] ❌ Canvas not found! Create Canvas first.");
            return;
        }

        // Create Button GameObject
        GameObject buttonObj = new GameObject("MobileInteractButton");
        Undo.RegisterCreatedObjectUndo(buttonObj, "Create MobileInteractButton");
        buttonObj.transform.SetParent(canvas.transform, false);

        // Add RectTransform and setup
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(-100, 100);
        rect.sizeDelta = new Vector2(120, 120);

        // Add Image component (background)
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent white

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Add MobileInteractButton script
        buttonObj.AddComponent<MobileInteractButton>();

        // Create Text child
        GameObject textObj = new GameObject("Text");
        Undo.RegisterCreatedObjectUndo(textObj, "Create Button Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = "E";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 40;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        Debug.Log("[UISetupBuilder] ✅ Created MobileInteractButton");
    }

    static void CreateCodeInputPanel()
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[UISetupBuilder] ❌ Canvas not found! Create Canvas first.");
            return;
        }

        // Create Panel GameObject
        GameObject panelObj = new GameObject("CodeInputPanel");
        Undo.RegisterCreatedObjectUndo(panelObj, "Create CodeInputPanel");
        panelObj.transform.SetParent(canvas.transform, false);

        // Setup RectTransform (centered, 400x300)
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(400, 300);

        // Add background Image
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark semi-transparent

        // Create InputField
        GameObject inputFieldObj = new GameObject("CodeInputField");
        Undo.RegisterCreatedObjectUndo(inputFieldObj, "Create InputField");
        inputFieldObj.transform.SetParent(panelObj.transform, false);

        RectTransform inputRect = inputFieldObj.AddComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.1f, 0.6f);
        inputRect.anchorMax = new Vector2(0.9f, 0.8f);
        inputRect.sizeDelta = Vector2.zero;

        Image inputImage = inputFieldObj.AddComponent<Image>();
        inputImage.color = Color.white;

        InputField inputField = inputFieldObj.AddComponent<InputField>();
        
        // Create Text child for InputField
        GameObject inputTextObj = new GameObject("Text");
        Undo.RegisterCreatedObjectUndo(inputTextObj, "Create InputField Text");
        inputTextObj.transform.SetParent(inputFieldObj.transform, false);

        RectTransform inputTextRect = inputTextObj.AddComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = new Vector2(10, 5);
        inputTextRect.offsetMax = new Vector2(-10, -5);

        Text inputText = inputTextObj.AddComponent<Text>();
        inputText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        inputText.fontSize = 24;
        inputText.color = Color.black;

        inputField.textComponent = inputText;

        // Create Submit Button
        GameObject submitBtnObj = new GameObject("SubmitButton");
        Undo.RegisterCreatedObjectUndo(submitBtnObj, "Create Submit Button");
        submitBtnObj.transform.SetParent(panelObj.transform, false);

        RectTransform submitRect = submitBtnObj.AddComponent<RectTransform>();
        submitRect.anchorMin = new Vector2(0.3f, 0.2f);
        submitRect.anchorMax = new Vector2(0.7f, 0.4f);
        submitRect.sizeDelta = Vector2.zero;

        Image submitImage = submitBtnObj.AddComponent<Image>();
        submitImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green

        Button submitButton = submitBtnObj.AddComponent<Button>();

        // Submit button text
        GameObject submitTextObj = new GameObject("Text");
        Undo.RegisterCreatedObjectUndo(submitTextObj, "Create Submit Text");
        submitTextObj.transform.SetParent(submitBtnObj.transform, false);

        RectTransform submitTextRect = submitTextObj.AddComponent<RectTransform>();
        submitTextRect.anchorMin = Vector2.zero;
        submitTextRect.anchorMax = Vector2.one;
        submitTextRect.sizeDelta = Vector2.zero;

        Text submitText = submitTextObj.AddComponent<Text>();
        submitText.text = "Submit";
        submitText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        submitText.fontSize = 20;
        submitText.alignment = TextAnchor.MiddleCenter;
        submitText.color = Color.white;

        // Create Feedback Text
        GameObject feedbackTextObj = new GameObject("FeedbackText");
        Undo.RegisterCreatedObjectUndo(feedbackTextObj, "Create Feedback Text");
        feedbackTextObj.transform.SetParent(panelObj.transform, false);

        RectTransform feedbackRect = feedbackTextObj.AddComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.1f, 0.45f);
        feedbackRect.anchorMax = new Vector2(0.9f, 0.55f);
        feedbackRect.sizeDelta = Vector2.zero;

        Text feedbackText = feedbackTextObj.AddComponent<Text>();
        feedbackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        feedbackText.fontSize = 18;
        feedbackText.alignment = TextAnchor.MiddleCenter;
        feedbackText.color = Color.red;

        // Add CodeInputUI script to Panel
        CodeInputUI codeInputUI = panelObj.AddComponent<CodeInputUI>();

        // Properly assign serialized fields using Unity's Editor API (not reflection)
        SerializedObject serializedObject = new SerializedObject(codeInputUI);
        serializedObject.FindProperty("panel").objectReferenceValue = panelObj;
        serializedObject.FindProperty("codeInputField").objectReferenceValue = inputField;
        serializedObject.FindProperty("submitButton").objectReferenceValue = submitButton;
        serializedObject.FindProperty("feedbackText").objectReferenceValue = feedbackText;
        serializedObject.ApplyModifiedProperties();

        // DON'T set inactive here! CodeInputUI.Awake() needs to run in Play mode.
        // GameObject must start ACTIVE so Awake() can set the static instance, 
        // then Awake() will hide the panel itself (line 31 in CodeInputUI.cs)

        Debug.Log("[UISetupBuilder] ✅ Created CodeInputPanel");
    }
}
#endif
