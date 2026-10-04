using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class LevelBuilderHelper : EditorWindow
{
    [MenuItem("Tools/MirrorHouse/Auto Build Role C Level")]
    public static void BuildLevel()
    {
        // 1. San nha tong the
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor_Ground";
        floor.transform.position = new Vector3(10f, 0f, 5f);
        floor.transform.localScale = new Vector3(35f, 0.2f, 35f);

        // 2. Tao 7 Waypoints cho AI
        Vector3[] wpPositions = new Vector3[]
        {
            new Vector3(6f, 1f, 8f),     // WP_01
            new Vector3(18f, 1f, 12f),   // WP_02
            new Vector3(12f, 1f, 0f),    // WP_03
            new Vector3(12f, 1.5f, 5f),  // WP_04
            new Vector3(12f, 1f, 8f),    // WP_05
            new Vector3(10f, 1f, 5f),    // WP_06
            new Vector3(8f, 1f, 3f)      // WP_07
        };

        GameObject wpGroup = new GameObject("Waypoints_Group");
        for (int i = 0; i < wpPositions.Length; i++)
        {
            GameObject wp = new GameObject("WP_0" + (i + 1));
            wp.transform.position = wpPositions[i];
            wp.transform.parent = wpGroup.transform;
        }

        // 3. Tao Player & Camera
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(8f, 1f, 8f);
        player.tag = "Player";
        player.AddComponent(typeof(CharacterController));
        player.AddComponent(System.Type.GetType("PlayerController") ?? typeof(MonoBehaviour));

        GameObject camHolder = new GameObject("CameraHolder");
        camHolder.transform.parent = player.transform;
        camHolder.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        camHolder.AddComponent(typeof(Camera));
        camHolder.AddComponent(typeof(AudioListener));
        camHolder.AddComponent(System.Type.GetType("PlayerCamera") ?? typeof(MonoBehaviour));

        // Xoa Main Camera cu neu co
        Camera oldCam = Camera.main;
        if (oldCam != null && oldCam.gameObject != camHolder)
        {
            DestroyImmediate(oldCam.gameObject);
        }

        // 4. Tao Enemy
        GameObject enemy = new GameObject("Enemy");
        enemy.transform.position = new Vector3(10f, 1f, 5f);
        NavMeshAgent agent = (NavMeshAgent)enemy.AddComponent(typeof(NavMeshAgent));
        agent.speed = 3.5f;

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.parent = enemy.transform;
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.8f, 1.8f, 0.8f);
        enemy.AddComponent(System.Type.GetType("AIController") ?? typeof(MonoBehaviour));

        // 5. Tao Puzzle Objects - TYPE-SAFE + CONFIGURED
        // 5.1 Safe Bedroom (Code lock: 1234)
        GameObject safe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        safe.name = "Safe_Bedroom";
        safe.transform.position = new Vector3(8f, 1f, 6f);
        safe.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        Rigidbody safeRb = safe.AddComponent<Rigidbody>();
        safeRb.isKinematic = true;
        Lockable safeLock = safe.AddComponent<Lockable>();
        SerializedObject soSafe = new SerializedObject(safeLock);
        soSafe.FindProperty("lockType").enumValueIndex = 1; // Code
        soSafe.FindProperty("lockId").stringValue = "safe_bedroom";
        soSafe.FindProperty("correctCode").stringValue = "1234";
        soSafe.ApplyModifiedProperties();

        // 5.2 Clue Anniversary (hints safe code)
        GameObject cluePhoto = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cluePhoto.name = "Clue_AnniversaryPhoto";
        cluePhoto.transform.position = new Vector3(7.5f, 1.5f, 4f);
        Collider cluePhotoCol = cluePhoto.GetComponent<Collider>();
        if (cluePhotoCol != null) cluePhotoCol.isTrigger = true;
        CodeClue photoClue = cluePhoto.AddComponent<CodeClue>();
        SerializedObject soPhoto = new SerializedObject(photoClue);
        soPhoto.FindProperty("clueId").stringValue = "clue_anniversary";
        soPhoto.FindProperty("clueText").stringValue = "Our anniversary: 12/34";
        soPhoto.ApplyModifiedProperties();

        // 5.3 Cabinet Bathroom (needs Master key)
        GameObject cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cab.name = "Cabinet_Bathroom";
        cab.transform.position = new Vector3(18f, 1f, 12f);
        cab.transform.localScale = new Vector3(1f, 1.5f, 0.5f);
        Rigidbody cabRb = cab.AddComponent<Rigidbody>();
        cabRb.isKinematic = true;
        Lockable cabLock = cab.AddComponent<Lockable>();
        SerializedObject soCab = new SerializedObject(cabLock);
        soCab.FindProperty("lockType").enumValueIndex = 0; // Key
        soCab.FindProperty("lockId").stringValue = "cabinet_bathroom";
        soCab.FindProperty("requiredKeyId").stringValue = "key_master";
        soCab.ApplyModifiedProperties();

        // 5.4 Key Brass (unlocks Kitchen door)
        GameObject keyBrass = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        keyBrass.name = "Key_Brass";
        keyBrass.transform.position = new Vector3(18f, 1.5f, 12f);
        keyBrass.transform.localScale = new Vector3(0.2f, 0.05f, 0.2f);
        Collider keyBrassCol = keyBrass.GetComponent<Collider>();
        if (keyBrassCol != null) keyBrassCol.isTrigger = true;
        KeyItem brassItem = keyBrass.AddComponent<KeyItem>();
        SerializedObject soBrass = new SerializedObject(brassItem);
        soBrass.FindProperty("keyId").stringValue = "key_brass";
        soBrass.ApplyModifiedProperties();

        // 5.5 Clue Kitchen (hints about master key)
        GameObject clueKitchen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        clueKitchen.name = "Clue_KitchenRecipe";
        clueKitchen.transform.position = new Vector3(18f, 2f, 12f);
        Collider clueKitchenCol = clueKitchen.GetComponent<Collider>();
        if (clueKitchenCol != null) clueKitchenCol.isTrigger = true;
        CodeClue kitchenClue = clueKitchen.AddComponent<CodeClue>();
        SerializedObject soKitchen = new SerializedObject(kitchenClue);
        soKitchen.FindProperty("clueId").stringValue = "clue_kitchen";
        soKitchen.FindProperty("clueText").stringValue = "Check the garden for the master key";
        soKitchen.ApplyModifiedProperties();

        // 5.6 Door Kitchen (needs Brass key)
        GameObject doorKit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        doorKit.name = "Door_Kitchen";
        doorKit.transform.position = new Vector3(20f, 1f, 10f);
        doorKit.transform.localScale = new Vector3(1.2f, 2.2f, 0.1f);
        Rigidbody dkRb = doorKit.AddComponent<Rigidbody>();
        dkRb.isKinematic = true;
        Lockable kitchenDoorLock = doorKit.AddComponent<Lockable>();
        SerializedObject soDoorKit = new SerializedObject(kitchenDoorLock);
        soDoorKit.FindProperty("lockType").enumValueIndex = 0; // Key
        soDoorKit.FindProperty("lockId").stringValue = "door_kitchen";
        soDoorKit.FindProperty("requiredKeyId").stringValue = "key_brass";
        soDoorKit.ApplyModifiedProperties();

        // 5.7 Key Master (unlocks Cabinet & Exit)
        GameObject keyMaster = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        keyMaster.name = "Key_Master";
        keyMaster.transform.position = new Vector3(22f, 1f, 9f);
        keyMaster.transform.localScale = new Vector3(0.2f, 0.05f, 0.2f);
        Collider keyMasterCol = keyMaster.GetComponent<Collider>();
        if (keyMasterCol != null) keyMasterCol.isTrigger = true;
        KeyItem masterItem = keyMaster.AddComponent<KeyItem>();
        SerializedObject soMaster = new SerializedObject(masterItem);
        soMaster.FindProperty("keyId").stringValue = "key_master";
        soMaster.ApplyModifiedProperties();

        // 5.8 Main Exit Door (needs Master key)
        GameObject exitDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        exitDoor.name = "Door_MainExit";
        exitDoor.transform.position = new Vector3(10f, 1f, -5f);
        exitDoor.transform.localScale = new Vector3(1.5f, 2.5f, 0.2f);
        Rigidbody exitRb = exitDoor.AddComponent<Rigidbody>();
        exitRb.isKinematic = true;
        Lockable exitLock = exitDoor.AddComponent<Lockable>();
        SerializedObject soExit = new SerializedObject(exitLock);
        soExit.FindProperty("lockType").enumValueIndex = 0; // Key
        soExit.FindProperty("lockId").stringValue = "door_exit";
        soExit.FindProperty("requiredKeyId").stringValue = "key_master";
        soExit.ApplyModifiedProperties();

        EditorUtility.DisplayDialog("Thành công", "Đã tạo đầy đủ Level và Puzzle theo đúng thiết kế của Role C!", "OK");
    }
}