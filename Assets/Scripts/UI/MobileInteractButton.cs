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
        playerController = FindAnyObjectByType<PlayerController>();
        
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
        
        // Đảm bảo nút E luôn cân đối, nằm an toàn bên trong màn hình điện thoại
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(-190f, 190f);
            rt.sizeDelta = new Vector2(120f, 120f);
        }

        // Trang trí nút theo phong cách Gothic Horror
        HorrorUITheme.ApplyHorrorButtonStyle(interactButton, 
            new Color(0.12f, 0.08f, 0.10f, 0.85f), 
            HorrorUITheme.ColorBoxBorderGold);

        Text txt = GetComponentInChildren<Text>();
        if (txt != null)
        {
            txt.color = HorrorUITheme.TextCandleGold;
            txt.fontStyle = FontStyle.Bold;
            txt.fontSize = 42;
            txt.alignment = TextAnchor.MiddleCenter;
            HorrorUITheme.AddHorrorTextShadow(txt, Color.black, new Vector2(2f, -2f));
        }

        // LUÔN HIỆN BUTTON - Để test trong Game View và hoạt động trên Mobile
        gameObject.SetActive(true);
        Debug.Log("[MobileInteractButton] Button configured at (-190, 190) - Safe inside screen");
        
        // Register button click event
        interactButton.onClick.RemoveListener(OnInteractButtonClick);
        interactButton.onClick.AddListener(OnInteractButtonClick);
    }
    
    void OnInteractButtonClick()
    {
        // First try interacting with the exact object in the crosshairs
        CameraRaycastInteraction raycastInteraction = FindAnyObjectByType<CameraRaycastInteraction>();
        if (raycastInteraction != null && raycastInteraction.InteractWithCurrent())
        {
            Debug.Log("[MobileInteractButton] Successfully interacted via CameraRaycastInteraction!");
            return;
        }

        if (playerController != null)
        {
            Debug.Log("[MobileInteractButton] Button clicked → calling playerController.TryInteract()");
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
