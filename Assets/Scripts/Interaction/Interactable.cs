using UnityEngine;
using System;

/// <summary>
/// BASE CLASS - Hệ thống tương tác generic cho tất cả objects có thể interact
/// 
/// ROLE C (Level Designer):
/// - Không dùng class này trực tiếp! Dùng Lockable, KeyItem, CodeClue
/// - Chỉ config interactionRange và interactionPrompt trong Inspector
/// 
/// ROLE D (UI/SFX):
/// - Subscribe events: OnPlayerEnterRange, OnPlayerExitRange
/// - Override ShowPrompt()/HidePrompt() để hiện UI
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("ROLE C: Khoảng cách player có thể interact (mét)")]
    [SerializeField] protected float interactionRange = 2f;
    
    [Tooltip("ROLE C: Text hiển thị khi player gần (Role D sẽ dùng làm UI)")]
    [SerializeField] protected string interactionPrompt = "Press E to interact";

    protected Transform player;
    protected bool playerInRange = false;

    // ROLE D: Subscribe events này để hiện/ẩn UI prompt
    /// <summary>Trigger khi player bước vào interaction range</summary>
    public event Action OnPlayerEnterRange;
    /// <summary>Trigger khi player ra khỏi interaction range</summary>
    public event Action OnPlayerExitRange;

    protected virtual void Start()
    {
        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Interactable: Không tìm thấy GameObject với tag 'Player'!");
        }
    }

    protected virtual void Update()
    {
        CheckPlayerDistance();
    }

    protected void CheckPlayerDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool inRange = distance <= interactionRange;

        if (inRange && !playerInRange)
        {
            playerInRange = true;
            OnPlayerEnterRange?.Invoke();
            ShowPrompt();
        }
        else if (!inRange && playerInRange)
        {
            playerInRange = false;
            OnPlayerExitRange?.Invoke();
            HidePrompt();
        }
    }

    public string InteractionPrompt => interactionPrompt;

    /// <summary>
    /// Kiểm tra xem đối tượng có thể tương tác lúc này hay không.
    /// Ví dụ: nếu nằm trong một ngăn kéo (DrawerInteractive) đang đóng thì không thể tương tác xuyên qua gỗ.
    /// </summary>
    public virtual bool CanInteract()
    {
        DrawerInteractive parentDrawer = GetComponentInParent<DrawerInteractive>();
        // Chỉ chặn nếu parentDrawer là một ngăn kéo cha khác VÀ ngăn kéo đó đang đóng
        // (Nếu chính object này là DrawerInteractive thì phải luôn tương tác được để mở/đóng)
        if (parentDrawer != null && parentDrawer != this && !parentDrawer.isOpen)
        {
            return false;
        }
        return true;
    }

    public void TryInteract()
    {
        if (!CanInteract()) return;
        Interact();
    }

    protected abstract void Interact();

    protected virtual void ShowPrompt()
    {
        Debug.Log($"[{gameObject.name}] {interactionPrompt}");
    }

    protected virtual void HidePrompt()
    {
        Debug.Log($"[{gameObject.name}] Hide interaction prompt");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}