using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// DOOR SETUP TOOL - Tự động setup DoorHinge cho tất cả cửa trong scene
/// 
/// ROLE A (Programmer):
/// Tool này tự động tạo và gắn DoorHinge cho mọi cửa có DoorInteractive
/// 
/// CÁCH DÙNG:
/// 1. Mở scene Level_Main
/// 2. Menu: Tools → Fix All Doors In Scene
/// 3. Tool sẽ tự động fix tất cả cửa thiếu doorHinge
/// 
/// HOẠT ĐỘNG:
/// - Tìm tất cả GameObject có component DoorInteractive
/// - Với mỗi cửa thiếu doorHinge:
///   + Tạo Empty GameObject "DoorHinge" làm con
///   + Đặt hinge ở cạnh trái của cửa (dựa vào bounds)
///   + Di chuyển model cửa xuống dưới hinge
///   + Gắn reference vào DoorInteractive.doorHinge
/// - Lưu scene
/// </summary>
public class DoorSetupTool : EditorWindow
{
    [MenuItem("Tools/Fix All Doors In Scene")]
    public static void FixAllDoors()
    {
        // Tìm tất cả DoorInteractive trong scene
        DoorInteractive[] doors = FindObjectsOfType<DoorInteractive>();
        
        if (doors.Length == 0)
        {
            Debug.LogWarning("[DoorSetupTool] Không tìm thấy cửa nào trong scene!");
            EditorUtility.DisplayDialog("Door Setup Tool", 
                "Không tìm thấy cửa nào có component DoorInteractive trong scene.", "OK");
            return;
        }
        
        Debug.Log($"[DoorSetupTool] Tìm thấy {doors.Length} cửa. Bắt đầu fix...");
        
        int fixedCount = 0;
        int skippedCount = 0;
        
        foreach (DoorInteractive door in doors)
        {
            if (FixSingleDoor(door))
            {
                fixedCount++;
            }
            else
            {
                skippedCount++;
            }
        }
        
        // Mark scene dirty để Unity biết cần save
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        string message = $"✅ Hoàn thành!\n\n" +
                        $"• Fixed: {fixedCount} cửa\n" +
                        $"• Skipped: {skippedCount} cửa (đã có hinge)\n\n" +
                        $"Nhớ SAVE scene (Ctrl+S)!";
        
        Debug.Log($"[DoorSetupTool] {message}");
        EditorUtility.DisplayDialog("Door Setup Tool", message, "OK");
    }
    
    /// <summary>
    /// Fix một cửa cụ thể
    /// Returns true nếu đã fix, false nếu skip (đã có hinge)
    /// </summary>
    private static bool FixSingleDoor(DoorInteractive door)
    {
        string doorName = door.gameObject.name;
        
        // Kiểm tra xem đã có doorHinge chưa
        Transform existingHinge = door.GetType()
            .GetField("doorHinge", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(door) as Transform;
        
        if (existingHinge != null)
        {
            Debug.Log($"[DoorSetupTool] SKIP: {doorName} - Đã có doorHinge");
            return false;
        }
        
        Debug.Log($"[DoorSetupTool] FIXING: {doorName}...");
        
        // Tìm hoặc tạo DoorHinge
        Transform hinge = door.transform.Find("DoorHinge");
        
        if (hinge == null)
        {
            // Tạo DoorHinge mới
            GameObject hingeObj = new GameObject("DoorHinge");
            hinge = hingeObj.transform;
            hinge.SetParent(door.transform, false);
            
            Debug.Log($"  → Đã tạo DoorHinge cho {doorName}");
        }
        else
        {
            Debug.Log($"  → Sử dụng DoorHinge có sẵn cho {doorName}");
        }
        
        // Tính toán vị trí hinge dựa vào bounds của door model
        PositionHingeAtDoorEdge(door.transform, hinge);
        
        // Di chuyển các child objects (model, collider) vào dưới hinge
        ReparentDoorModel(door.transform, hinge);
        
        // Gắn hinge vào DoorInteractive component qua Reflection
        var hingeField = door.GetType()
            .GetField("doorHinge", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (hingeField != null)
        {
            hingeField.SetValue(door, hinge);
            EditorUtility.SetDirty(door);
            Debug.Log($"  ✅ {doorName} - Fixed successfully!");
        }
        else
        {
            Debug.LogError($"  ❌ {doorName} - Không thể gắn doorHinge (field not found)");
        }
        
        return true;
    }
    
    /// <summary>
    /// Đặt hinge ở cạnh trái của cửa dựa vào bounds
    /// </summary>
    private static void PositionHingeAtDoorEdge(Transform door, Transform hinge)
    {
        // Lấy bounds của tất cả renderer trong door
        Renderer[] renderers = door.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
        {
            // Không có renderer, đặt hinge ở vị trí door
            hinge.localPosition = Vector3.zero;
            Debug.LogWarning($"    ⚠️ {door.name} - Không có Renderer, đặt hinge ở center");
            return;
        }
        
        // Tính combined bounds trong local space của door
        Bounds localBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool initialized = false;
        
        foreach (Renderer renderer in renderers)
        {
            // Chuyển bounds của renderer về local space của door
            Bounds rendererBounds = renderer.bounds;
            Vector3 center = door.InverseTransformPoint(rendererBounds.center);
            Vector3 extents = door.InverseTransformVector(rendererBounds.extents);
            
            Bounds localRendererBounds = new Bounds(center, extents * 2);
            
            if (!initialized)
            {
                localBounds = localRendererBounds;
                initialized = true;
            }
            else
            {
                localBounds.Encapsulate(localRendererBounds);
            }
        }
        
        // Xác định cạnh nào là bản lề (cạnh trái hoặc cạnh dưới)
        // Giả sử cửa rộng hơn theo X hoặc Z
        bool widthIsX = localBounds.size.x >= localBounds.size.z;
        
        Vector3 hingePosition;
        if (widthIsX)
        {
            // Cửa rộng theo X → hinge ở cạnh trái (X min)
            hingePosition = new Vector3(
                localBounds.min.x,
                localBounds.min.y,
                localBounds.center.z
            );
        }
        else
        {
            // Cửa rộng theo Z → hinge ở cạnh dưới (Z min)
            hingePosition = new Vector3(
                localBounds.center.x,
                localBounds.min.y,
                localBounds.min.z
            );
        }
        
        hinge.localPosition = hingePosition;
        hinge.localRotation = Quaternion.identity;
        
        Debug.Log($"    → Hinge positioned at local {hingePosition} (bounds: {localBounds.size})");
    }
    
    /// <summary>
    /// Di chuyển các child objects (trừ DoorHinge) vào dưới hinge
    /// </summary>
    private static void ReparentDoorModel(Transform door, Transform hinge)
    {
        List<Transform> childrenToMove = new List<Transform>();
        
        // Collect children (trừ hinge)
        foreach (Transform child in door)
        {
            if (child != hinge)
            {
                childrenToMove.Add(child);
            }
        }
        
        // Move children under hinge, giữ world position
        foreach (Transform child in childrenToMove)
        {
            child.SetParent(hinge, true);
            Debug.Log($"    → Moved {child.name} under DoorHinge");
        }
        
        // Di chuyển collider từ door root xuống hinge (nếu có)
        BoxCollider rootCollider = door.GetComponent<BoxCollider>();
        if (rootCollider != null)
        {
            // Copy collider properties
            bool isTrigger = rootCollider.isTrigger;
            Vector3 center = rootCollider.center;
            Vector3 size = rootCollider.size;
            PhysicsMaterial material = rootCollider.sharedMaterial;
            
            // Destroy root collider
            Object.DestroyImmediate(rootCollider);
            
            // Create collider on hinge
            BoxCollider hingeCollider = hinge.gameObject.AddComponent<BoxCollider>();
            hingeCollider.center = center - hinge.localPosition;
            hingeCollider.size = size;
            hingeCollider.isTrigger = isTrigger;
            hingeCollider.sharedMaterial = material;
            
            Debug.Log($"    → Moved BoxCollider from root to DoorHinge");
        }
    }
}
