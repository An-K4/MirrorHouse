using UnityEngine;
using System.Collections;

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
    [Tooltip("Transform trục xoay của cánh cửa (nếu để trống sẽ tự lấy transform của chính object này)")]
    [SerializeField] private Transform doorHinge;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotateCoroutine;

    void Start()
    {
        if (doorHinge == null)
            doorHinge = transform;

        closedRotation = doorHinge.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (startsOpen)
        {
            isOpen = true;
            doorHinge.localRotation = openRotation;
        }

        UpdatePrompt();
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
