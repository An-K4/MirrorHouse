using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MOBILE INTERACT BUTTON - Nút tương tác cho mobile (thay thế phím E)
/// 
/// ROLE A (Programmer):
/// 1. Gắn script này vào GameObject chứa Button UI
/// 2. Assign Button component vào Inspector
/// 3. Button chỉ hiện trên Android/iOS, ẩn trên PC
/// 
/// HOẠT ĐỘNG:
/// - Tìm PlayerController trong scene
/// - Khi nhấn button → gọi PlayerController.TryInteract()
/// - Tự động ẩn/hiện dựa vào platform (Android/iOS = hiện, PC = ẩn)
/// </summary>
public class MobileInteractButton : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo Button component vào đây")]
    [SerializeField] private Button interactButton;
    
    private PlayerController playerController;
    
    void Start()
    {
        // Tìm PlayerController trong scene
        playerController = FindObjectOfType<PlayerController>();
        
        if (playerController == null)
        {
            Debug.LogError("[MobileInteractButton] Không tìm thấy PlayerController trong scene!");
        }
        
        // Kiểm tra có Button component không
        if (interactButton == null)
        {
            interactButton = GetComponent<Button>();
        }
        
        if (interactButton == null)
        {
            Debug.LogError("[MobileInteractButton] Không có Button component!");
            return;
        }
        
        // LUÔN HIỆN BUTTON - Để test trong Game View và hoạt động trên Mobile
        // User có thể test bằng cách click chuột trong Unity Editor
        gameObject.SetActive(true);
        Debug.Log("[MobileInteractButton] Button enabled - Ready for testing");
        
        // Register button click event
        interactButton.onClick.AddListener(OnInteractButtonClick);
    }
    
    void OnInteractButtonClick()
    {
        if (playerController != null)
        {
            Debug.Log("[MobileInteractButton] Button clicked → calling TryInteract()");
            playerController.TryInteract();
        }
        else
        {
            Debug.LogWarning("[MobileInteractButton] PlayerController is null! Cannot interact.");
        }
    }
    
    void OnDestroy()
    {
        // Cleanup: Remove listener khi destroy
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(OnInteractButtonClick);
        }
    }
}
