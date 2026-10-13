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
    
    // AUTO-CLEAR INVENTORY khi game start (fix static field persistence bug)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetInventory()
    {
        playerInventory.Clear();
        Debug.Log("[INVENTORY] Cleared inventory on game start");
    }
    
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
    
    [Header("Win Condition")]
    [Tooltip("Tích chọn nếu đây là cửa thoát hiểm / gương thoát (Mở khóa này = Thắng game)")]
    [SerializeField] private bool isExitTrigger = false;
    
    [Header("Reward Settings")]
    [Tooltip("Chìa khóa hoặc vật phẩm tự động nhận khi mở khóa (VD: key_library, key_storage, key_ritual_dagger)")]
    [SerializeField] private string rewardKeyId = "";
    [Tooltip("Object vật lý xuất hiện khi mở khóa (nếu có)")]
    [SerializeField] private GameObject rewardObject = null;
    [Tooltip("Tên hiển thị trên giao diện nhập mã (VD: KÉT SẮT PHÒNG NGỦ)")]
    [SerializeField] private string lockDisplayName = "";

    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(lockDisplayName))
            return lockDisplayName;

        return lockId switch
        {
            "safe_bedroom" => "KÉT SẮT PHÒNG NGỦ",
            "safe_library" => "KÉT BÍ MẬT THƯ VIỆN",
            "chest_storage" => "KÉT KIM LOẠI NHÀ KHO",
            "altar_ritual" => "BÀN THỜ TẾ NGHI LỄ",
            "exit_gateway_mirror" => "GƯƠNG THẦN THOÁT HIỂM",
            _ => "KÉT SẮT BÍ MẬT"
        };
    }

    public string GetRewardKeyId()
    {
        if (!string.IsNullOrEmpty(rewardKeyId))
            return rewardKeyId;

        return lockId switch
        {
            "safe_bedroom" => "key_library",
            "safe_library" => "key_storage",
            "chest_storage" => "key_ritual_dagger",
            _ => ""
        };
    }
    
    private bool isLocked = true;
    
    protected override void Interact()
    {
        if (!isLocked)
        {
            if (isExitTrigger || lockId == "exit_gateway_mirror" || lockId == "door_main")
            {
                GameEvents.TriggerGameWin();
            }
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
            // Code lock → Show UI nhập mã
            CodeInputUI.Show(this);
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
    public bool TryUnlockWithCode(string inputCode)
    {
        if (inputCode == correctCode)
        {
            GameEvents.TriggerCodeCorrect(lockId);
            Unlock();
            return true;
        }
        else
        {
            Debug.Log("Wrong code!");
            return false;
        }
    }
    
    void Unlock()
    {
        isLocked = false;
        GameEvents.TriggerLockOpened(lockId);
        Debug.Log($"Unlocked: {lockId}");

        // Tự động trao thưởng chìa khóa / vật phẩm tương ứng
        string grantKey = GetRewardKeyId();
        if (!string.IsNullOrEmpty(grantKey))
        {
            AddKeyToInventory(grantKey);
            GameEvents.TriggerKeyPickup(grantKey);
            Debug.Log($"[LOCKABLE] Đã nhận được vật phẩm: {grantKey}");
        }

        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
        }

        if (isExitTrigger || lockId == "exit_gateway_mirror" || lockId == "door_main")
        {
            GameEvents.TriggerGameWin();
        }

        Open();
    }
    
    void Open()
    {
        Debug.Log($"Opening: {lockId}");
        gameObject.SetActive(false); // Tạm thời ẩn đi
    }
    
    // Getters cho Role D
    public LockType GetLockType() => lockType;
    public string GetLockId() => lockId;
    public string GetCorrectCode() => correctCode;
    
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
