// GameEvents.cs
// Event system để kết nối giữa Role A (logic) và Role D (UI/Audio)
// Role A - Sprint 1

using System;
using UnityEngine;

public static class GameEvents
{
    // ============ PLAYER EVENTS ============
    
    /// <summary>
    /// Trigger khi player bị AI bắt
    /// Role D subscribe để hiện UI game over + SFX
    /// </summary>
    public static event Action OnPlayerCaught;
    
    /// <summary>
    /// Trigger khi player thoát được nhà (thắng game)
    /// Role D subscribe để hiện UI victory + SFX
    /// </summary>
    public static event Action OnGameWin;
    
    /// <summary>
    /// Trigger khi player thay đổi trạng thái sprint
    /// Role D subscribe để thay đổi footstep sound
    /// </summary>
    public static event Action<bool> OnPlayerSprintChanged;
    
    // ============ INTERACTION EVENTS ============
    
    /// <summary>
    /// Trigger khi nhập đúng mã khóa
    /// Params: lockId (string) - ID của khóa đã mở
    /// Role D subscribe để phát SFX + hiệu ứng mở cửa
    /// </summary>
    public static event Action<string> OnCodeCorrect;
    
    /// <summary>
    /// Trigger khi nhặt chìa khóa
    /// Params: keyId (string) - ID của chìa khóa đã nhặt
    /// Role D subscribe để cập nhật UI inventory + SFX
    /// </summary>
    public static event Action<string> OnKeyPickup;
    
    /// <summary>
    /// Trigger khi nhặt gợi ý mã số
    /// Params: clueId (string), clueText (string)
    /// Role D subscribe để hiện UI popup + lưu vào notebook
    /// </summary>
    public static event Action<string, string> OnCluePickup;
    
    /// <summary>
    /// Trigger khi mở khóa thành công (bất kể loại gì)
    /// Params: lockId (string)
    /// Role D subscribe để phát SFX unlock
    /// </summary>
    public static event Action<string> OnLockOpened;
    
    // ============ HELPER METHODS (Role A sẽ gọi) ============
    
    public static void TriggerPlayerCaught()
    {
        OnPlayerCaught?.Invoke();
        Debug.Log("[GameEvents] Player bị bắt!");
    }
    
    public static void TriggerGameWin()
    {
        OnGameWin?.Invoke();
        Debug.Log("[GameEvents] Player thắng!");
    }
    
    public static void TriggerCodeCorrect(string lockId)
    {
        OnCodeCorrect?.Invoke(lockId);
        Debug.Log($"[GameEvents] Nhập đúng mã khóa: {lockId}");
    }
    
    public static void TriggerKeyPickup(string keyId)
    {
        OnKeyPickup?.Invoke(keyId);
        Debug.Log($"[GameEvents] Nhặt chìa khóa: {keyId}");
    }
    
    public static void TriggerCluePickup(string clueId, string clueText)
    {
        OnCluePickup?.Invoke(clueId, clueText);
        Debug.Log($"[GameEvents] Nhặt gợi ý: {clueId} - {clueText}");
    }
    
    public static void TriggerLockOpened(string lockId)
    {
        OnLockOpened?.Invoke(lockId);
        Debug.Log($"[GameEvents] Mở khóa thành công: {lockId}");
    }
}
