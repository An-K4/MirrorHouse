using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DrawerInteractive : Interactable
{
    [Header("Drawer Settings")]
    [Tooltip("Direction to slide in WORLD SPACE. Look at Scene view:\nNorth (+Z) = (0,0,1)\nSouth (-Z) = (0,0,-1)\nEast (+X) = (1,0,0)\nWest (-X) = (-1,0,0)")]
    public Vector3 slideDirection = new Vector3(0, 0, -1);
    
    [Tooltip("How far the drawer slides out when opened (in meters)")]
    public float slideDistance = 0.3f;
    
    [Tooltip("Thời gian trượt mở / đóng ngăn kéo (giây). Mở mượt mà trong 0.35s")]
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float slideSpeed = 1.2f;
    public bool isOpen;

    [Header("Lock")]
    public bool isLocked;
    public string requiredKeyId = "";
    public string lockedMessage = "Ngăn kéo bị khóa!";

    [Header("Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip lockedSound;

    [Header("Events")]
    public UnityEvent<string> onLockedMessage;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Coroutine slideRoutine;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();

        // Đảm bảo collider của ngăn kéo là vật thể cứng, không phải Trigger để Raycast bắt dính 100%
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null && col.isTrigger)
        {
            col.isTrigger = false;
        }
        else if (col == null && GetComponent<Collider>() == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }

        // Đảm bảo khoảng cách trượt mở tối thiểu 0.35m để thấy rõ ngăn kéo kéo ra
        if (slideDistance < 0.35f) slideDistance = 0.35f;

        // Store initial position as closed position
        closedPosition = transform.localPosition;
        
        // Calculate open position using WORLD SPACE direction
        // slideDirection is in world space - works for any desk rotation!
        Vector3 worldSlideDir = slideDirection != Vector3.zero ? slideDirection.normalized : new Vector3(0, 0, 1);
        
        // Calculate world positions
        Vector3 closedWorldPos = transform.position;
        Vector3 openWorldPos = closedWorldPos + worldSlideDir * slideDistance;
        
        // Convert back to local position relative to parent
        if (transform.parent != null)
        {
            openPosition = transform.parent.InverseTransformPoint(openWorldPos);
        }
        else
        {
            openPosition = openWorldPos;
        }

        Debug.Log($"<color=cyan>[DRAWER] Sẵn sàng ngăn kéo '{gameObject.name}' - Hướng trượt: {worldSlideDir}, Cự ly: {slideDistance}m</color>");
        
        // Get or add AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            audioSource.maxDistance = 10f;
        }
        
        // If drawer should start open, set to open position
        if (isOpen) transform.localPosition = openPosition;

        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (isLocked)
        {
            interactionPrompt = "Ngăn kéo bị khóa (Click / E để thử mở)";
        }
        else
        {
            interactionPrompt = isOpen ? "Click / E để đóng ngăn kéo" : "Click / E để kéo ngăn kéo";
        }
    }

    protected override void Interact()
    {
        if (isLocked)
        {
            if (!string.IsNullOrEmpty(requiredKeyId) && Lockable.HasKey(requiredKeyId))
            {
                isLocked = false;
                Debug.Log($"<color=green>[DRAWER] Đã dùng chìa khóa '{requiredKeyId}' mở khóa ngăn kéo thành công!</color>");
                GameEvents.TriggerLockOpened(requiredKeyId);
                ToggleDrawer();
            }
            else
            {
                if (lockedSound != null && audioSource != null) audioSource.PlayOneShot(lockedSound);
                onLockedMessage?.Invoke(lockedMessage);
                Debug.Log($"<color=yellow>[DRAWER] {lockedMessage}</color>");
            }
            UpdatePrompt();
            return;
        }
        ToggleDrawer();
    }

    public void ToggleDrawer()
    {
        isOpen = !isOpen;
        UpdatePrompt();

        var clip = isOpen ? openSound : closeSound;
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);

        if (slideRoutine != null) StopCoroutine(slideRoutine);
        Vector3 target = isOpen ? openPosition : closedPosition;
        Debug.Log($"<color=cyan>[DRAWER] {gameObject.name} → {(isOpen ? "MỞ NGĂN KÉO (OPENING)" : "ĐÓNG NGĂN KÉO (CLOSING)")} - Khoảng cách: {slideDistance}m</color>");
        slideRoutine = StartCoroutine(Slide(target));
    }

    IEnumerator Slide(Vector3 target)
    {
        Vector3 start = transform.localPosition;
        float elapsed = 0f;
        float duration = slideDuration > 0.01f ? slideDuration : 0.35f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            transform.localPosition = Vector3.Lerp(start, target, smoothT);
            yield return null;
        }

        transform.localPosition = target;
        slideRoutine = null;
        Debug.Log($"<color=cyan>[DRAWER] {gameObject.name} → {(isOpen ? "ĐÃ MỞ HOÀN TẤT (OPENED)" : "ĐÃ ĐÓNG HOÀN TẤT (CLOSED)")}</color>");
    }

    public override bool CanInteract()
    {
        // Ngăn kéo luôn luôn có thể tương tác (để kéo mở ra hoặc đóng lại)
        return true;
    }

    public void Unlock() => isLocked = false;
}
