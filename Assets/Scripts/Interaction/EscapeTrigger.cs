using UnityEngine;

/// <summary>
/// ESCAPE TRIGGER - Vùng kích hoạt chiến thắng khi người chơi trốn thoát
/// 
/// Đặt component này vào GameObject có BoxCollider (IsTrigger = true).
/// Khi Player bước vào vùng này (hoặc sau khi đã mở khóa cửa chính / gương),
/// trigger sẽ kích hoạt GameEvents.TriggerGameWin().
/// </summary>
[RequireComponent(typeof(Collider))]
public class EscapeTrigger : MonoBehaviour
{
    [Header("Escape Settings")]
    [Tooltip("Khóa cần mở trước khi có thể trốn thoát qua vùng này (để trống nếu không cần)")]
    [SerializeField] private string requiredKeyId = "";

    [Tooltip("Thông báo nếu chưa đủ điều kiện trốn thoát")]
    [SerializeField] private string blockedMessage = "Cần mở khóa cửa chính để thoát!";

    private bool escaped = false;

    private void OnTriggerEnter(Collider other)
    {
        if (escaped) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            if (!string.IsNullOrEmpty(requiredKeyId) && !Lockable.HasKey(requiredKeyId))
            {
                Debug.Log($"[EscapeTrigger] {blockedMessage}");
                return;
            }

            escaped = true;
            Debug.Log("[EscapeTrigger] Người chơi đã trốn thoát thành công!");
            GameEvents.TriggerGameWin();
        }
    }
}
