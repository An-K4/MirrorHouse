using UnityEngine;

/// <summary>
/// Performs raycast from camera center to detect and interact with drawers
/// Works alongside CrosshairUI to provide visual feedback
/// Press E to interact with drawer in crosshair
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraRaycastInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Maximum distance to detect interactable objects")]
    public float interactionRange = 3f;
    
    [Tooltip("Layer mask for raycast (what can be detected)")]
    public LayerMask interactionLayer = ~0; // Default: all layers
    
    [Header("Input Settings")]
    [Tooltip("Key to press for interaction")]
    public KeyCode interactKey = KeyCode.E;
    
    [Header("UI References")]
    [Tooltip("Crosshair UI to update based on raycast")]
    public CrosshairUI crosshairUI;
    
    [Header("Debug")]
    [Tooltip("Show raycast debug line in Scene view")]
    public bool showDebugRay = true;
    
    // Internal state
    private Camera mainCamera;
    private DrawerInteractive currentDrawer;
    
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        
        if (mainCamera == null)
        {
            Debug.LogError("[CameraRaycastInteraction] No Camera component found!");
            enabled = false;
            return;
        }
        
        // Find crosshair UI if not assigned
        if (crosshairUI == null)
        {
            crosshairUI = FindObjectOfType<CrosshairUI>();
            if (crosshairUI == null)
            {
                Debug.LogWarning("[CameraRaycastInteraction] No CrosshairUI found in scene");
            }
        }
        
        Debug.Log("[CameraRaycastInteraction] Initialized - Range: " + interactionRange + "m");
    }
    
    void Update()
    {
        PerformRaycast();
        HandleInput();
    }
    
    /// <summary>
    /// Perform raycast from screen center to detect drawers
    /// </summary>
    private void PerformRaycast()
    {
        // Ray from camera center (screen center)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
        // Perform raycast
        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
        {
            // Check if we hit a drawer
            DrawerInteractive drawer = hit.collider.GetComponent<DrawerInteractive>();
            
            if (drawer != null)
            {
                // Looking at a drawer
                if (currentDrawer != drawer)
                {
                    currentDrawer = drawer;
                    OnDrawerEnter(drawer);
                }
            }
            else
            {
                // Hit something but not a drawer
                if (currentDrawer != null)
                {
                    OnDrawerExit();
                }
            }
            
            // Debug visualization
            if (showDebugRay)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
            }
        }
        else
        {
            // Didn't hit anything
            if (currentDrawer != null)
            {
                OnDrawerExit();
            }
            
            // Debug visualization
            if (showDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
            }
        }
    }
    
    /// <summary>
    /// Handle player input for interaction
    /// </summary>
    private void HandleInput()
    {
        // Check if E key pressed and we're looking at a drawer
        if (Input.GetKeyDown(interactKey) && currentDrawer != null)
        {
            Debug.Log($"[CameraRaycastInteraction] Interacting with {currentDrawer.gameObject.name}");
            currentDrawer.Interact();
        }
    }
    
    /// <summary>
    /// Called when raycast starts hitting a drawer
    /// </summary>
    private void OnDrawerEnter(DrawerInteractive drawer)
    {
        Debug.Log($"[CameraRaycastInteraction] Looking at: {drawer.gameObject.name}");
        
        // Update crosshair to highlight
        if (crosshairUI != null)
        {
            crosshairUI.SetHighlight();
        }
    }
    
    /// <summary>
    /// Called when raycast stops hitting a drawer
    /// </summary>
    private void OnDrawerExit()
    {
        if (currentDrawer != null)
        {
            Debug.Log($"[CameraRaycastInteraction] Stopped looking at: {currentDrawer.gameObject.name}");
        }
        
        currentDrawer = null;
        
        // Update crosshair to normal
        if (crosshairUI != null)
        {
            crosshairUI.SetNormal();
        }
    }
    
    /// <summary>
    /// For debugging - show interaction range in Scene view
    /// </summary>
    void OnDrawGizmos()
    {
        if (!enabled || mainCamera == null)
            return;
        
        // Draw interaction range sphere
        Gizmos.color = Color.cyan;
        Vector3 forward = mainCamera.transform.forward;
        Gizmos.DrawWireSphere(mainCamera.transform.position + forward * interactionRange, 0.1f);
    }
}
