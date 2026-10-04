using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// AUTO-BUILD DEMO SCENE FOR PRESENTATION WEEK 3 - TOPIC #7
/// Tạo scene demo với 5 khu vực interaction types được organize rõ ràng
/// 
/// Usage: Tools -> MirrorHouse -> Create Demo Topic 7 Scene
/// </summary>
public class DemoSceneBuilder : EditorWindow
{
    [MenuItem("Tools/MirrorHouse/Create Demo Topic 7 Scene")]
    static void BuildDemoScene()
    {
        // 1. Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // 2. Setup lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.4f);
        
        // 3. Setup camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = new Vector3(0, 10, -15);
            mainCam.transform.rotation = Quaternion.Euler(30, 0, 0);
            mainCam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        }
        
        // 4. Create floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(10, 1, 10);
        
        // 5. Create demo areas (5 areas in a line, spacing = 15 units)
        CreateArea1_KeyAndDoor(new Vector3(-30, 0, 0));
        CreateArea2_CodeLockAndClue(new Vector3(-15, 0, 0));
        CreateArea3_CabinetContainer(new Vector3(0, 0, 0));
        CreateArea4_ReadableObjects(new Vector3(15, 0, 0));
        CreateArea5_Peephole(new Vector3(30, 0, 0));
        
        // 6. Save scene
        string scenePath = "Assets/Scenes/Demo_Topic7_Interaction.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        EditorUtility.DisplayDialog(
            "Demo Scene Created!", 
            "Demo_Topic7_Interaction.unity created successfully!\n\n5 demo areas ready for presentation.\n\nNext: Test each area and replace primitives with B's models when PR #2 merges.",
            "OK"
        );
    }
    
    // ========== AREA 1: KEY + DOOR SYSTEM ==========
    static void CreateArea1_KeyAndDoor(Vector3 basePos)
    {
        // Label
        CreateTextLabel("1. KEY & DOOR INTERACTION", basePos + new Vector3(0, 3, 0));
        
        // Key (pickable)
        GameObject key = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        key.name = "Demo_Key_Area1";
        key.transform.position = basePos + new Vector3(-2, 0.5f, 0);
        key.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        key.GetComponent<Renderer>().material.color = Color.yellow;
        
        KeyItem keyItem = key.AddComponent<KeyItem>();
        SerializedObject soKey = new SerializedObject(keyItem);
        soKey.FindProperty("keyId").stringValue = "demo_key_area1";
        soKey.FindProperty("interactionPrompt").stringValue = "Press E to pick up Key";
        soKey.ApplyModifiedProperties();
        
        // Door (lockable)
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = "Demo_Door_Area1";
        door.transform.position = basePos + new Vector3(2, 1.5f, 0);
        door.transform.localScale = new Vector3(1.5f, 3f, 0.2f);
        door.GetComponent<Renderer>().material.color = new Color(0.6f, 0.3f, 0.1f); // Brown
        
        Rigidbody doorRb = door.AddComponent<Rigidbody>();
        doorRb.isKinematic = true;
        
        Lockable doorLock = door.AddComponent<Lockable>();
        SerializedObject soDoor = new SerializedObject(doorLock);
        soDoor.FindProperty("lockType").enumValueIndex = 0; // Key
        soDoor.FindProperty("lockId").stringValue = "demo_door_area1";
        soDoor.FindProperty("requiredKeyId").stringValue = "demo_key_area1";
        soDoor.FindProperty("interactionPrompt").stringValue = "Press E to unlock Door";
        soDoor.ApplyModifiedProperties();
        
        // Info marker
        CreateInfoMarker("1. Pick up yellow key\n2. Unlock brown door", basePos + new Vector3(0, 0.5f, 3));
    }
    
    // ========== AREA 2: CODE LOCK + CLUE ==========
    static void CreateArea2_CodeLockAndClue(Vector3 basePos)
    {
        // Label
        CreateTextLabel("2. CODE LOCK & CLUE SYSTEM", basePos + new Vector3(0, 3, 0));
        
        // Safe (code lock)
        GameObject safe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        safe.name = "Demo_Safe_Area2";
        safe.transform.position = basePos + new Vector3(0, 1f, 0);
        safe.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        safe.GetComponent<Renderer>().material.color = Color.gray;
        
        Rigidbody safeRb = safe.AddComponent<Rigidbody>();
        safeRb.isKinematic = true;
        
        Lockable safeLock = safe.AddComponent<Lockable>();
        SerializedObject soSafe = new SerializedObject(safeLock);
        soSafe.FindProperty("lockType").enumValueIndex = 1; // Code
        soSafe.FindProperty("lockId").stringValue = "demo_safe_area2";
        soSafe.FindProperty("correctCode").stringValue = "1234";
        soSafe.FindProperty("interactionPrompt").stringValue = "Press E to enter code";
        soSafe.ApplyModifiedProperties();
        
        // Clue (hints code)
        GameObject clue = GameObject.CreatePrimitive(PrimitiveType.Quad);
        clue.name = "Demo_Clue_Area2";
        clue.transform.position = basePos + new Vector3(-3, 1.5f, 0);
        clue.transform.rotation = Quaternion.Euler(0, 90, 0);
        clue.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        clue.GetComponent<Renderer>().material.color = new Color(1f, 0.9f, 0.7f); // Paper color
        
        Collider clueCol = clue.GetComponent<Collider>();
        if (clueCol != null) clueCol.isTrigger = true;
        
        CodeClue clueScript = clue.AddComponent<CodeClue>();
        SerializedObject soClue = new SerializedObject(clueScript);
        soClue.FindProperty("clueId").stringValue = "demo_clue_area2";
        soClue.FindProperty("clueText").stringValue = "Safe code: 1234";
        soClue.FindProperty("interactionPrompt").stringValue = "Press E to read Clue";
        soClue.ApplyModifiedProperties();
        
        // Add visible 3D text on clue
        GameObject clueText = new GameObject("ClueText");
        clueText.transform.parent = clue.transform;
        clueText.transform.localPosition = new Vector3(0, 0, -0.01f);
        clueText.transform.localRotation = Quaternion.identity;
        TextMesh tm = clueText.AddComponent<TextMesh>();
        tm.text = "CLUE:\nSafe code\nis 1234";
        tm.fontSize = 40;
        tm.characterSize = 0.01f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.black;
        
        // Info marker
        CreateInfoMarker("1. Read yellow clue note\n2. Enter code 1234\n3. Open gray safe", basePos + new Vector3(0, 0.5f, 3));
    }
    
    // ========== AREA 3: CABINET + CONTAINER ==========
    static void CreateArea3_CabinetContainer(Vector3 basePos)
    {
        // Label
        CreateTextLabel("3. CONTAINER INTERACTION", basePos + new Vector3(0, 3, 0));
        
        // Master Key (for cabinet)
        GameObject masterKey = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        masterKey.name = "Demo_MasterKey_Area3";
        masterKey.transform.position = basePos + new Vector3(-3, 0.5f, 0);
        masterKey.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        masterKey.GetComponent<Renderer>().material.color = Color.red;
        
        KeyItem masterKeyItem = masterKey.AddComponent<KeyItem>();
        SerializedObject soMaster = new SerializedObject(masterKeyItem);
        soMaster.FindProperty("keyId").stringValue = "demo_master_key_area3";
        soMaster.FindProperty("interactionPrompt").stringValue = "Press E to pick up Master Key";
        soMaster.ApplyModifiedProperties();
        
        // Cabinet (lockable container)
        GameObject cabinet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabinet.name = "Demo_Cabinet_Area3";
        cabinet.transform.position = basePos + new Vector3(0, 1.2f, 0);
        cabinet.transform.localScale = new Vector3(1.2f, 2f, 0.8f);
        cabinet.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0.1f); // Dark brown
        
        Rigidbody cabRb = cabinet.AddComponent<Rigidbody>();
        cabRb.isKinematic = true;
        
        Lockable cabLock = cabinet.AddComponent<Lockable>();
        SerializedObject soCab = new SerializedObject(cabLock);
        soCab.FindProperty("lockType").enumValueIndex = 0; // Key
        soCab.FindProperty("lockId").stringValue = "demo_cabinet_area3";
        soCab.FindProperty("requiredKeyId").stringValue = "demo_master_key_area3";
        soCab.FindProperty("interactionPrompt").stringValue = "Press E to unlock Cabinet";
        soCab.ApplyModifiedProperties();
        
        // Info marker
        CreateInfoMarker("1. Pick up red master key\n2. Unlock dark cabinet", basePos + new Vector3(0, 0.5f, 3));
    }
    
    // ========== AREA 4: READABLE OBJECTS ==========
    static void CreateArea4_ReadableObjects(Vector3 basePos)
    {
        // Label
        CreateTextLabel("4. READABLE OBJECTS", basePos + new Vector3(0, 3, 0));
        
        // Document 1
        GameObject doc1 = GameObject.CreatePrimitive(PrimitiveType.Quad);
        doc1.name = "Demo_Document1_Area4";
        doc1.transform.position = basePos + new Vector3(-2, 1.5f, 0);
        doc1.transform.rotation = Quaternion.Euler(0, 0, 0);
        doc1.transform.localScale = new Vector3(0.6f, 0.8f, 1);
        doc1.GetComponent<Renderer>().material.color = Color.white;
        
        Collider doc1Col = doc1.GetComponent<Collider>();
        if (doc1Col != null) doc1Col.isTrigger = true;
        
        CodeClue doc1Clue = doc1.AddComponent<CodeClue>();
        SerializedObject soDoc1 = new SerializedObject(doc1Clue);
        soDoc1.FindProperty("clueId").stringValue = "demo_doc1";
        soDoc1.FindProperty("clueText").stringValue = "Document 1: Research notes about the house history";
        soDoc1.FindProperty("interactionPrompt").stringValue = "Press E to read Document";
        soDoc1.ApplyModifiedProperties();
        
        // Add visible 3D text
        GameObject doc1Text = new GameObject("Doc1Text");
        doc1Text.transform.parent = doc1.transform;
        doc1Text.transform.localPosition = new Vector3(0, 0, -0.01f);
        doc1Text.transform.localRotation = Quaternion.identity;
        TextMesh tm1 = doc1Text.AddComponent<TextMesh>();
        tm1.text = "DOCUMENT 1:\nResearch\nNotes";
        tm1.fontSize = 35;
        tm1.characterSize = 0.01f;
        tm1.anchor = TextAnchor.MiddleCenter;
        tm1.alignment = TextAlignment.Center;
        tm1.color = Color.black;
        
        // Document 2
        GameObject doc2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
        doc2.name = "Demo_Document2_Area4";
        doc2.transform.position = basePos + new Vector3(2, 1.5f, 0);
        doc2.transform.rotation = Quaternion.Euler(0, 0, 0);
        doc2.transform.localScale = new Vector3(0.6f, 0.8f, 1);
        doc2.GetComponent<Renderer>().material.color = new Color(0.9f, 0.9f, 0.7f);
        
        Collider doc2Col = doc2.GetComponent<Collider>();
        if (doc2Col != null) doc2Col.isTrigger = true;
        
        CodeClue doc2Clue = doc2.AddComponent<CodeClue>();
        SerializedObject soDoc2 = new SerializedObject(doc2Clue);
        soDoc2.FindProperty("clueId").stringValue = "demo_doc2";
        soDoc2.FindProperty("clueText").stringValue = "Document 2: Diary entry about hidden passages";
        soDoc2.FindProperty("interactionPrompt").stringValue = "Press E to read Diary";
        soDoc2.ApplyModifiedProperties();
        
        // Add visible 3D text
        GameObject doc2Text = new GameObject("Doc2Text");
        doc2Text.transform.parent = doc2.transform;
        doc2Text.transform.localPosition = new Vector3(0, 0, -0.01f);
        doc2Text.transform.localRotation = Quaternion.identity;
        TextMesh tm2 = doc2Text.AddComponent<TextMesh>();
        tm2.text = "DIARY:\nHidden\nPassages";
        tm2.fontSize = 35;
        tm2.characterSize = 0.01f;
        tm2.anchor = TextAnchor.MiddleCenter;
        tm2.alignment = TextAlignment.Center;
        tm2.color = new Color(0.3f, 0.2f, 0.1f); // Dark brown
        
        // Info marker
        CreateInfoMarker("Read white document (left)\nRead yellow diary (right)", basePos + new Vector3(0, 0.5f, 3));
    }
    
    // ========== AREA 5: DOOR PEEPHOLE (PLACEHOLDER) ==========
    static void CreateArea5_Peephole(Vector3 basePos)
    {
        // Label
        CreateTextLabel("5. PEEPHOLE VIEW (TODO)", basePos + new Vector3(0, 3, 0));
        
        // Door with peephole marker
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = "Demo_Door_Peephole_Area5";
        door.transform.position = basePos + new Vector3(0, 1.5f, 0);
        door.transform.localScale = new Vector3(1.5f, 3f, 0.2f);
        door.GetComponent<Renderer>().material.color = new Color(0.5f, 0.25f, 0.1f);
        
        // Peephole marker (small sphere)
        GameObject peephole = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        peephole.name = "Demo_Peephole_Marker";
        peephole.transform.position = basePos + new Vector3(0, 1.8f, 0.11f);
        peephole.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        peephole.GetComponent<Renderer>().material.color = Color.black;
        
        // Info marker
        CreateInfoMarker("Peephole script not implemented yet.\nWill add in Sprint 3.", basePos + new Vector3(0, 0.5f, 3));
    }
    
    // ========== HELPER METHODS ==========
    
    static void CreateTextLabel(string text, Vector3 position)
    {
        GameObject label = new GameObject("Label_" + text.Substring(0, Mathf.Min(10, text.Length)));
        label.transform.position = position;
        
        // Create 3D text using TextMesh
        TextMesh textMesh = label.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 50;
        textMesh.characterSize = 0.1f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.white;
        
        // Add background quad
        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bg.name = "LabelBackground";
        bg.transform.parent = label.transform;
        bg.transform.localPosition = new Vector3(0, 0, 0.1f);
        bg.transform.localScale = new Vector3(text.Length * 0.3f, 1f, 1f);
        bg.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.7f);
    }
    
    static void CreateInfoMarker(string text, Vector3 position)
    {
        GameObject marker = new GameObject("InfoMarker");
        marker.transform.position = position;
        
        // Cube marker
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Marker";
        cube.transform.parent = marker.transform;
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        cube.GetComponent<Renderer>().material.color = Color.cyan;
        
        // Text
        TextMesh textMesh = marker.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 30;
        textMesh.characterSize = 0.05f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.yellow;
    }
}
