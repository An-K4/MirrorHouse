using UnityEngine;

/// <summary>
/// KEY SYSTEM - Chìa khóa vật lý có thể nhặt
/// 
/// ROLE C (Level Designer):
/// 1. Đặt keyId UNIQUE và phải MATCH với Lockable.requiredKeyId
///    Naming convention: "key_blue", "key_red", "key_master"
/// 2. Gán keyIcon sprite cho UI (Role D sẽ hiển thị trong inventory)
/// 
/// ROLE D (UI/SFX):
/// - Subscribe GameEvents.OnKeyPickup(keyId) để:
///   + Add key icon vào inventory UI
///   + Play pickup sound/animation
/// </summary>
public class KeyItem : Interactable
{
    [Header("Key Settings")]
    [Tooltip("ROLE C: ID chìa khóa - phải MATCH với Lockable.requiredKeyId (VD: key_blue, key_red)")]
    [SerializeField] private string keyId = "key_blue";
    
    [Tooltip("ROLE C: Icon sprite cho inventory UI (Role D sẽ hiển thị)")]
    [SerializeField] private Sprite keyIcon;
    
    protected override void Interact()
    {
        PickupKey();
    }
    
    void PickupKey()
    {
        // Add to inventory
        Lockable.AddKeyToInventory(keyId);
        
        // Trigger event cho Role D
        GameEvents.TriggerKeyPickup(keyId);
        
        Debug.Log($"✓ Picked up key: {keyId}");
        
        // Destroy hoặc ẩn
        gameObject.SetActive(false);
    }
    
    public string GetKeyId() => keyId;
}
