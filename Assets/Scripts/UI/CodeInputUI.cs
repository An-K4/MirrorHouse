using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI PANEL - Nhập mã số cho Code Lock
/// 
/// Setup trong Unity:
/// 1. Tạo Canvas (nếu chưa có)
/// 2. Add child Panel với InputField và Button
/// 3. Gắn script này vào Panel
/// 4. Assign references trong Inspector
/// 
/// Usage: Lockable script sẽ gọi CodeInputUI.Show(lockable)
/// </summary>
public class CodeInputUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private InputField codeInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Text feedbackText;
    
    private Lockable currentLockable;
    private static CodeInputUI instance;
    
    void Awake()
    {
        instance = this;
        
        if (panel != null)
            panel.SetActive(false);
        
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitClicked);
    }
    
    void Update()
    {
        // Press Enter to submit
        if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmitClicked();
        }
        
        // Press Escape to cancel
        if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }
    
    /// <summary>Hiển thị UI nhập mã - gọi từ Lockable</summary>
    public static void Show(Lockable lockable)
    {
        if (instance == null)
        {
            Debug.LogError("CodeInputUI: Instance not found! Create UI Canvas first.");
            return;
        }
        
        instance.currentLockable = lockable;
        
        if (instance.panel != null)
        {
            instance.panel.SetActive(true);
            
            // Clear input field
            if (instance.codeInputField != null)
            {
                instance.codeInputField.text = "";
                instance.codeInputField.Select();
                instance.codeInputField.ActivateInputField();
            }
            
            // Clear feedback
            if (instance.feedbackText != null)
                instance.feedbackText.text = "";
            
            // Pause game (optional)
            Time.timeScale = 0f;
            
            // Unlock cursor for typing
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    void OnSubmitClicked()
    {
        if (currentLockable == null)
        {
            Debug.LogError("CodeInputUI: No lockable assigned!");
            return;
        }
        
        if (codeInputField == null)
        {
            Debug.LogError("CodeInputUI: InputField not assigned!");
            return;
        }
        
        string inputCode = codeInputField.text.Trim();
        
        if (string.IsNullOrEmpty(inputCode))
        {
            ShowFeedback("Please enter a code!");
            return;
        }
        
        // Try unlock
        currentLockable.TryUnlockWithCode(inputCode);
        
        // Check if unlocked (hacky way - better to use events)
        // For now, just hide UI after attempt
        Hide();
    }
    
    void ShowFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }
    
    void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        
        currentLockable = null;
        
        // Resume game
        Time.timeScale = 1f;
        
        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
