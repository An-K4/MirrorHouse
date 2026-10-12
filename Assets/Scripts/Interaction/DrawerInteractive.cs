using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DrawerInteractive : MonoBehaviour
{
    [Header("Drawer Settings")]
    [Tooltip("Direction to slide in WORLD SPACE. Look at Scene view:\nNorth (+Z) = (0,0,1)\nSouth (-Z) = (0,0,-1)\nEast (+X) = (1,0,0)\nWest (-X) = (-1,0,0)")]
    public Vector3 slideDirection = new Vector3(0, 0, -1);
    
    [Tooltip("How far the drawer slides out when opened (in meters)")]
    public float slideDistance = 0.3f;
    
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

    void Start()
    {
        // Store initial position as closed position
        closedPosition = transform.localPosition;
        
        // Calculate open position using WORLD SPACE direction
        // slideDirection is in world space - works for any desk rotation!
        Vector3 worldSlideDir = slideDirection.normalized;
        
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
    }

    public void Interact()
    {
        if (isLocked)
        {
            if (lockedSound != null) audioSource.PlayOneShot(lockedSound);
            onLockedMessage?.Invoke(lockedMessage);
            return;
        }
        ToggleDrawer();
    }

    public void ToggleDrawer()
    {
        isOpen = !isOpen;

        var clip = isOpen ? openSound : closeSound;
        if (clip != null) audioSource.PlayOneShot(clip);

        if (slideRoutine != null) StopCoroutine(slideRoutine);
        Vector3 target = isOpen ? openPosition : closedPosition;
        slideRoutine = StartCoroutine(Slide(target));
    }

    IEnumerator Slide(Vector3 target)
    {
        while ((transform.localPosition - target).sqrMagnitude > 0.000001f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, target, slideSpeed * Time.deltaTime);
            yield return null;
        }
        transform.localPosition = target;
        slideRoutine = null;
    }

    public void Unlock() => isLocked = false;
}
