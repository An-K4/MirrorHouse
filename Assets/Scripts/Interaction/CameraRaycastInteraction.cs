using UnityEngine;

/// <summary>
/// Performs a raycast from camera center to detect and interact with scene objects.
/// Works alongside CrosshairUI to provide visual feedback
/// Press E to interact with the object in the crosshair.
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
    private Interactable currentInteractable;
    private MirrorHouseHorrorHUD horrorHUD;

    public Interactable CurrentInteractable => currentInteractable;

    public bool InteractWithCurrent()
    {
        if (currentInteractable != null && currentInteractable.CanInteract())
        {
            Debug.Log($"[CameraRaycastInteraction] Interacting with {currentInteractable.gameObject.name}");
            currentInteractable.TryInteract();
            return true;
        }
        return false;
    }
    
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
            crosshairUI = FindAnyObjectByType<CrosshairUI>();
            if (crosshairUI == null)
            {
                Debug.LogWarning("[CameraRaycastInteraction] No CrosshairUI found in scene");
            }
        }

        horrorHUD = MirrorHouseHorrorHUD.EnsureExists();
        
        Debug.Log("[CameraRaycastInteraction] Initialized - Range: " + interactionRange + "m");
    }
    
    void Update()
    {
        PerformRaycast();
        HandleInput();
    }
    
    /// <summary>
    /// Perform a raycast from screen center to detect interactable objects.
    /// </summary>
    private void PerformRaycast()
    {
        // Ray from camera center (screen center)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
        // Perform raycast (hỗ trợ cả trigger lẫn non-trigger colliders)
        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer, QueryTriggerInteraction.Collide))
        {
            // Check the collider and its parents so doors, drawers, clues and keys all work.
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            
            if (interactable != null && interactable.CanInteract())
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    OnInteractableEnter(interactable);
                }
            }
            else
            {
                if (currentInteractable != null)
                {
                    OnInteractableExit();
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
            if (currentInteractable != null)
            {
                OnInteractableExit();
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
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && currentInteractable.CanInteract())
        {
            Debug.Log($"[CameraRaycastInteraction] Interacting with {currentInteractable.gameObject.name}");
            currentInteractable.TryInteract();
        }
    }
    
    /// <summary>
    /// Called when raycast starts hitting an interactable object.
    /// </summary>
    private void OnInteractableEnter(Interactable interactable)
    {
        Debug.Log($"[CameraRaycastInteraction] Looking at: {interactable.gameObject.name}");
        
        // Update crosshair to highlight
        if (crosshairUI != null)
        {
            crosshairUI.SetHighlight();
        }

        if (horrorHUD != null)
        {
            horrorHUD.SetInteractionTarget(interactable);
        }
    }
    
    /// <summary>
    /// Called when raycast stops hitting an interactable object.
    /// </summary>
    private void OnInteractableExit()
    {
        if (currentInteractable != null)
        {
            Debug.Log($"[CameraRaycastInteraction] Stopped looking at: {currentInteractable.gameObject.name}");
        }
        
        currentInteractable = null;
        
        // Update crosshair to normal
        if (crosshairUI != null)
        {
            crosshairUI.SetNormal();
        }

        if (horrorHUD != null)
        {
            horrorHUD.SetInteractionTarget(null);
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
