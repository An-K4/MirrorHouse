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
    [MenuItem("MirrorHouse/1. Mở Map Game (Level_Main) Để Chỉnh Sửa", false, 0)]
    public static void OpenLevelMain()
    {
        string scenePath = "Assets/Scenes/Level_Main.unity";
        if (File.Exists(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath);
            Debug.Log("[MirrorHouse] ✅ Đã mở Map Level_Main thành công!");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Không tìm thấy Level_Main.unity tại: " + scenePath, "OK");
        }
    }

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

        // Create UI and Managers
        CreateCanvas();
        CreateEventSystem();
        CreateJoystick();
        CreateMobileInteractButton();
        CreateCodeInputPanel();
        CreateGameManagers();

        // Save scene
        EditorSceneManager.SaveScene(currentScene);
        
        Debug.Log("[UISetupBuilder] ✅ Complete UI & Game Loop setup finished for Level_Main!");
        EditorUtility.DisplayDialog("Success", "Thiết lập hoàn tất!\n\n- Canvas (Responsive 1920x1080)\n- Virtual Joystick\n- MobileInteractButton\n- CodeInputPanel\n- MirrorHouseGameManager\n- AudioManager (Procedural SFX/BGM)\n- GameUIManager (HUD, Inventory, Pause, Game Over, Victory)", "OK");
    }

    static void CreateGameManagers()
    {
        // 1. MirrorHouseGameManager
        var existingGM = GameObject.FindObjectOfType<MirrorHouseGameManager>();
        if (existingGM == null)
        {
            GameObject gmObj = new GameObject("MirrorHouseGameManager");
            Undo.RegisterCreatedObjectUndo(gmObj, "Create MirrorHouseGameManager");
            gmObj.AddComponent<MirrorHouseGameManager>();
            Debug.Log("[UISetupBuilder] ✅ Created MirrorHouseGameManager");
        }

        // 2. AudioManager
        var existingAudio = GameObject.FindObjectOfType<AudioManager>();
        if (existingAudio == null)
        {
            GameObject audioObj = new GameObject("AudioManager");
            Undo.RegisterCreatedObjectUndo(audioObj, "Create AudioManager");
            audioObj.AddComponent<AudioManager>();
            Debug.Log("[UISetupBuilder] ✅ Created AudioManager");
        }

        // 3. GameUIManager
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            var existingUI = canvas.GetComponentInChildren<GameUIManager>();
            if (existingUI == null)
            {
                GameObject uiManagerObj = new GameObject("GameUIManager");
                Undo.RegisterCreatedObjectUndo(uiManagerObj, "Create GameUIManager");
                uiManagerObj.transform.SetParent(canvas.transform, false);
                uiManagerObj.AddComponent<GameUIManager>();
                Debug.Log("[UISetupBuilder] ✅ Created GameUIManager");
            }
        }
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

    static void CreateEventSystem()
    {
        // Check if EventSystem already exists
        var existingEventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (existingEventSystem != null)
        {
            Debug.Log("[UISetupBuilder] ✅ EventSystem already exists");
            return;
        }

        // Create EventSystem GameObject
        GameObject eventSystemObj = new GameObject("EventSystem");
        Undo.RegisterCreatedObjectUndo(eventSystemObj, "Create EventSystem");

        // Add EventSystem component
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();

        // Add StandaloneInputModule (handles keyboard/mouse input)
        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        Debug.Log("[UISetupBuilder] ✅ Created EventSystem");
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

        // Position joystick at bottom-left (cân đối, không bị cắt viền)
        RectTransform rect = joystick.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(170, 170);
            rect.sizeDelta = new Vector2(200, 200);
        }

        Transform handleTrans = joystick.transform.Find("Handle");
        if (handleTrans != null)
        {
            RectTransform handleRect = handleTrans.GetComponent<RectTransform>();
            if (handleRect != null)
            {
                handleRect.sizeDelta = new Vector2(85, 85);
            }
        }

        // Tự động gán Joystick vào PlayerController
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        if (player != null)
        {
            SerializedObject so = new SerializedObject(player);
            SerializedProperty prop = so.FindProperty("joystick");
            if (prop != null)
            {
                prop.objectReferenceValue = joystick.GetComponent<Joystick>();
                so.ApplyModifiedProperties();
                Debug.Log("[UISetupBuilder] ✅ Đã tự động gán Joystick vào PlayerController!");
            }
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
        submitRect.anchorMin = new Vector2(0.1f, 0.15f);
        submitRect.anchorMax = new Vector2(0.48f, 0.35f);
        submitRect.sizeDelta = Vector2.zero;

        Image submitImage = submitBtnObj.AddComponent<Image>();
        submitImage.color = new Color(0.2f, 0.75f, 0.2f, 1f); // Green

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
        submitText.text = "Nhập mã";
        submitText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        submitText.fontSize = 20;
        submitText.alignment = TextAnchor.MiddleCenter;
        submitText.color = Color.white;

        // Create Cancel Button
        GameObject cancelBtnObj = new GameObject("CancelButton");
        Undo.RegisterCreatedObjectUndo(cancelBtnObj, "Create Cancel Button");
        cancelBtnObj.transform.SetParent(panelObj.transform, false);

        RectTransform cancelRect = cancelBtnObj.AddComponent<RectTransform>();
        cancelRect.anchorMin = new Vector2(0.52f, 0.15f);
        cancelRect.anchorMax = new Vector2(0.9f, 0.35f);
        cancelRect.sizeDelta = Vector2.zero;

        Image cancelImage = cancelBtnObj.AddComponent<Image>();
        cancelImage.color = new Color(0.65f, 0.2f, 0.2f, 1f); // Red

        Button cancelButton = cancelBtnObj.AddComponent<Button>();

        // Cancel button text
        GameObject cancelTextObj = new GameObject("Text");
        Undo.RegisterCreatedObjectUndo(cancelTextObj, "Create Cancel Text");
        cancelTextObj.transform.SetParent(cancelBtnObj.transform, false);

        RectTransform cancelTextRect = cancelTextObj.AddComponent<RectTransform>();
        cancelTextRect.anchorMin = Vector2.zero;
        cancelTextRect.anchorMax = Vector2.one;
        cancelTextRect.sizeDelta = Vector2.zero;

        Text cancelText = cancelTextObj.AddComponent<Text>();
        cancelText.text = "Đóng";
        cancelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        cancelText.fontSize = 20;
        cancelText.alignment = TextAnchor.MiddleCenter;
        cancelText.color = Color.white;

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
        feedbackText.color = Color.yellow;

        // Add CodeInputUI script to Panel
        CodeInputUI codeInputUI = panelObj.AddComponent<CodeInputUI>();

        // Properly assign serialized fields using Unity's Editor API
        SerializedObject serializedObject = new SerializedObject(codeInputUI);
        serializedObject.FindProperty("panel").objectReferenceValue = panelObj;
        serializedObject.FindProperty("codeInputField").objectReferenceValue = inputField;
        serializedObject.FindProperty("submitButton").objectReferenceValue = submitButton;
        serializedObject.FindProperty("cancelButton").objectReferenceValue = cancelButton;
        serializedObject.FindProperty("feedbackText").objectReferenceValue = feedbackText;
        serializedObject.ApplyModifiedProperties();

        Debug.Log("[UISetupBuilder] ✅ Created CodeInputPanel");
    }

    [MenuItem("MirrorHouse/Create Main Menu Scene & Configure Build Settings", false, 3)]
    public static void CreateMainMenuScene()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Add Main Camera to scene
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.02f, 0.03f, 1f);
        camObj.tag = "MainCamera";

        GameObject root = new GameObject("MainMenuController");
        root.AddComponent<MainMenuUI>();

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[UISetupBuilder] ✅ Created MainMenu scene at " + scenePath);

        // Update Build Settings: MainMenu (index 0), Level_Main (index 1)
        var buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        string mainScenePath = "Assets/Scenes/Level_Main.unity";
        if (File.Exists(mainScenePath))
        {
            buildScenes.Add(new EditorBuildSettingsScene(mainScenePath, true));
        }
        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log("[UISetupBuilder] ✅ Configured EditorBuildSettings: MainMenu (0), Level_Main (1)");

        EditorUtility.DisplayDialog("Main Menu Created", "Đã tạo scene MainMenu và thiết lập Build Settings thành công!\n\n0: Assets/Scenes/MainMenu.unity\n1: Assets/Scenes/Level_Main.unity", "OK");
    }
}
#endif
