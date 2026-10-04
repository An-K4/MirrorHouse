using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DOOR INTERACTION SYSTEM
/// Cửa 4 bức tường kết nối các phòng. Click chuột hoặc nhấn E để mở/đóng cửa.
/// Có hỗ trợ khóa bằng chìa khóa (KeyId).
/// </summary>
public class DoorInteractive : Interactable
{
    [Header("Door Settings")]
    [Tooltip("Góc mở của cánh cửa (thường là 90 độ)")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private bool startsOpen = false;

    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyId = "";
    [SerializeField] private string lockedMessage = "Cửa đang bị khóa! Cần chìa khóa phù hợp.";

    [Header("Hinge / Pivot")]
    [Tooltip("Transform trục xoay của cánh cửa (nếu để trống, bản lề sẽ được tính từ bounds của model)")]
    [SerializeField] private Transform doorHinge;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotateCoroutine;

    void Start()
    {
        if (doorHinge == null)
            doorHinge = CreateRuntimeHingeFromModelBounds();

        closedRotation = doorHinge.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (startsOpen)
        {
            isOpen = true;
            doorHinge.localRotation = openRotation;
        }

        UpdatePrompt();
    }

    private Transform CreateRuntimeHingeFromModelBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return transform;

        Bounds localBounds = new Bounds();
        bool initialized = false;
        foreach (Renderer renderer in renderers)
        {
            Bounds rendererBounds = renderer.localBounds;
            Vector3 min = rendererBounds.min;
            Vector3 max = rendererBounds.max;
            for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
            for (int z = 0; z < 2; z++)
            {
                Vector3 corner = transform.InverseTransformPoint(renderer.transform.TransformPoint(new Vector3(
                    x == 0 ? min.x : max.x,
                    y == 0 ? min.y : max.y,
                    z == 0 ? min.z : max.z)));
                if (!initialized)
                {
                    localBounds = new Bounds(corner, Vector3.zero);
                    initialized = true;
                }
                else
                {
                    localBounds.Encapsulate(corner);
                }
            }
        }

        bool widthIsX = localBounds.size.x >= localBounds.size.z;
        float halfWidth = (widthIsX ? localBounds.size.x : localBounds.size.z) * 0.5f;
        // Scene instances created before calibrated prefabs may have a source pivot that
        // leaves the panel off-center or below/above its doorway. Keep the root at the
        // intended center-bottom anchor while moving the whole visual/collider assembly.
        transform.position += transform.TransformVector(new Vector3(
            -localBounds.center.x,
            -localBounds.min.y,
            -localBounds.center.z));

        GameObject hingeObject = new GameObject("DoorHinge_Runtime");
        Transform hinge = hingeObject.transform;
        hinge.SetParent(transform, false);
        hinge.localPosition = widthIsX
            ? new Vector3(localBounds.center.x - halfWidth, localBounds.min.y, localBounds.center.z)
            : new Vector3(localBounds.center.x, localBounds.min.y, localBounds.center.z - halfWidth);

        // Move visible model roots under the hinge without changing their world pose.
        List<Transform> modelRoots = new List<Transform>();
        foreach (Transform child in transform)
            modelRoots.Add(child);
        foreach (Transform child in modelRoots)
            child.SetParent(hinge, true);

        // The blocking collider must swing with the mesh. Convert a root box collider
        // into an equivalent collider on the hinge child.
        BoxCollider rootBox = GetComponent<BoxCollider>();
        if (rootBox != null)
        {
            bool isTrigger = rootBox.isTrigger;
            PhysicMaterial material = rootBox.sharedMaterial;
            Destroy(rootBox);

            BoxCollider hingeBox = hingeObject.AddComponent<BoxCollider>();
            hingeBox.size = new Vector3(
                Mathf.Max(0.01f, localBounds.size.x),
                Mathf.Max(0.01f, localBounds.size.y),
                Mathf.Max(0.01f, localBounds.size.z));
            hingeBox.center = localBounds.center - hinge.localPosition;
            hingeBox.isTrigger = isTrigger;
            hingeBox.sharedMaterial = material;
        }

        return hinge;
    }

    void UpdatePrompt()
    {
        if (isLocked)
        {
            interactionPrompt = "Cửa đã khóa (Click / E để thử mở)";
        }
        else
        {
            interactionPrompt = isOpen ? "Click / E để đóng cửa" : "Click / E để mở cửa";
        }
    }

    protected override void Interact()
    {
        if (isLocked)
        {
            if (!string.IsNullOrEmpty(requiredKeyId) && Lockable.HasKey(requiredKeyId))
            {
                isLocked = false;
                Debug.Log($"<color=green>[DOOR] Đã dùng chìa khóa '{requiredKeyId}' mở khóa cửa thành công!</color>");
                GameEvents.TriggerLockOpened(requiredKeyId);
                ToggleDoor();
            }
            else
            {
                Debug.Log($"<color=yellow>[DOOR] {lockedMessage}</color>");
            }
            UpdatePrompt();
            return;
        }

        ToggleDoor();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        UpdatePrompt();

        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(AnimateDoor(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator AnimateDoor(Quaternion targetRot)
    {
        while (Quaternion.Angle(doorHinge.localRotation, targetRot) > 0.5f)
        {
            doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, targetRot, Time.deltaTime * openSpeed);
            yield return null;
        }
        doorHinge.localRotation = targetRot;
    }

    public void Unlock()
    {
        isLocked = false;
        UpdatePrompt();
    }

    public void SetLocked(bool locked, string keyId = "")
    {
        isLocked = locked;
        requiredKeyId = keyId;
        UpdatePrompt();
    }
}
