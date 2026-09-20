using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// LOCK/DOOR SYSTEM - Cửa/tủ có thể khóa bằng Key hoặc Code
/// 
/// ROLE C (Level Designer):
/// 1. Chọn LockType: Key (cần chìa) hoặc Code (cần mã số)
/// 2. Đặt lockId UNIQUE (VD: "door_main", "chest_bedroom", "safe_01")
/// 3. Key Lock: Điền requiredKeyId phải MATCH với KeyItem.keyId
/// 4. Code Lock: Điền correctCode (VD: "1234", "9527")
/// 
/// ROLE D (UI/SFX):
/// - Subscribe GameEvents: OnCodeCorrect, OnLockOpened
/// - Call TryUnlockWithCode(string) từ UI code input
/// - GetLockType(), GetLockId() để customize UI
/// </summary>
public enum LockType
{
    Key,    // Cần chìa khóa vật lý
    Code    // Cần nhập mã số
}

public class Lockable : Interactable
{
    // SIMPLE INVENTORY SYSTEM - Track picked keys globally
    private static HashSet<string> playerInventory = new HashSet<string>();
    
    [Header("Lock Settings")]
    [Tooltip("ROLE C: Loại khóa - Key (chìa vật lý) hoặc Code (mã số)")]
    [SerializeField] private LockType lockType = LockType.Key;
    
    [Tooltip("ROLE C: ID UNIQUE cho lock này (VD: door_main, chest_bedroom, safe_01)")]
    [SerializeField] private string lockId = "door_main";
    
    [Header("Key Lock Settings")]
    [Tooltip("ROLE C: Chìa nào mở được? Phải MATCH với KeyItem.keyId (VD: key_blue, key_red)")]
    [SerializeField] private string requiredKeyId = "key_blue";
    
    [Header("Code Lock Settings")]
    [Tooltip("ROLE C: Mã số đúng (VD: 1234, 9527). Player phải nhập đúng mã này")]
    [SerializeField] private string correctCode = "1234";
    
    private bool isLocked = true;
    
    protected override void Interact()
    {
        if (!isLocked)
        {
            Open();
            return;
        }
        
        // Nếu đang khóa
        if (lockType == LockType.Key)
        {
            TryUnlockWithKey();
        }
        else
        {
            // Code lock → Role D sẽ hiện UI nhập mã
            Debug.Log($"Code lock detected. Role D will show code input UI.");
        }
    }
    
    void TryUnlockWithKey()
    {
        // Check player inventory
        bool hasKey = playerInventory.Contains(requiredKeyId);
        
        if (hasKey)
        {
            Debug.Log($"✓ Using key: {requiredKeyId}");
            Unlock();
        }
        else
        {
            Debug.Log($"❌ Locked! Need key: {requiredKeyId}");
        }
    }
    
    /// <summary>
    /// ROLE D: Gọi method này từ UI code input khi player submit mã
    /// Example: lockable.TryUnlockWithCode(inputField.text);
    /// </summary>
    public void TryUnlockWithCode(string inputCode)
    {
        if (inputCode == correctCode)
        {
            GameEvents.TriggerCodeCorrect(lockId);
            Unlock();
        }
        else
        {
            Debug.Log("Wrong code!");
        }
    }
    
    void Unlock()
    {
        isLocked = false;
        GameEvents.TriggerLockOpened(lockId);
        Debug.Log($"Unlocked: {lockId}");
        Open();
    }
    
    void Open()
    {
        Debug.Log($"Opening: {lockId}");
        // TODO: Animation/SFX (Role B/D)
        gameObject.SetActive(false); // Tạm thời ẩn đi
    }
    
    // Getters cho Role D
    public LockType GetLockType() => lockType;
    public string GetLockId() => lockId;
    
    // INVENTORY MANAGEMENT - Role A
    /// <summary>Add key to player inventory (called by KeyItem when picked up)</summary>
    public static void AddKeyToInventory(string keyId)
    {
        playerInventory.Add(keyId);
        Debug.Log($"[INVENTORY] Added key: {keyId}");
    }
    
    /// <summary>Check if player has specific key</summary>
    public static bool HasKey(string keyId)
    {
        return playerInventory.Contains(keyId);
    }
}
