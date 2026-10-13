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
    [SerializeField] private Button cancelButton;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Text titleText;
    
    private Lockable currentLockable;
    private static CodeInputUI instance;
    private bool isStyled = false;
    
    void Awake()
    {
        instance = this;
        
        if (panel != null)
            panel.SetActive(false);
        
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitClicked);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Cancel);

        ApplyHorrorTheme();
    }

    private void ApplyHorrorTheme()
    {
        if (isStyled || panel == null) return;
        isStyled = true;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 1. Box Panel styling
        RectTransform panelRt = panel.GetComponent<RectTransform>();
        if (panelRt != null)
        {
            panelRt.sizeDelta = new Vector2(480f, 340f);
        }

        Image panelImg = panel.GetComponent<Image>();
        if (panelImg != null)
        {
            panelImg.sprite = HorrorUITheme.GetRoundedBoxSprite(HorrorUITheme.ColorCryptDark, HorrorUITheme.ColorBoxBorderGold, 3.5f, 22);
            panelImg.type = Image.Type.Sliced;
            panelImg.color = Color.white;
        }

        // 2. Title Text
        if (titleText == null)
        {
            Transform tTr = panel.transform.Find("TitleText");
            if (tTr != null)
            {
                titleText = tTr.GetComponent<Text>();
            }
            else
            {
                GameObject tObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
                tObj.transform.SetParent(panel.transform, false);
                RectTransform tRt = tObj.GetComponent<RectTransform>();
                tRt.anchorMin = new Vector2(0f, 1f);
                tRt.anchorMax = new Vector2(1f, 1f);
                tRt.pivot = new Vector2(0.5f, 1f);
                tRt.anchoredPosition = new Vector2(0f, -22f);
                tRt.sizeDelta = new Vector2(440f, 45f);

                titleText = tObj.GetComponent<Text>();
                titleText.font = font;
                titleText.fontSize = 24;
                titleText.fontStyle = FontStyle.Bold;
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = HorrorUITheme.TextCandleGold;
                titleText.text = "KÉT SẮT CỔ";
                HorrorUITheme.AddHorrorTextOutline(titleText, new Color(0.12f, 0.01f, 0.02f, 0.95f), new Vector2(2f, -2f));
                HorrorUITheme.AddHorrorTextShadow(titleText, Color.black, new Vector2(1.5f, -1.5f));
            }
        }

        // 3. InputField styling
        if (codeInputField != null)
        {
            RectTransform inputRt = codeInputField.GetComponent<RectTransform>();
            inputRt.sizeDelta = new Vector2(340f, 56f);
            inputRt.anchoredPosition = new Vector2(0f, 15f);

            Image inputImg = codeInputField.GetComponent<Image>();
            if (inputImg != null)
            {
                inputImg.sprite = HorrorUITheme.GetRoundedBoxSprite(new Color(0.12f, 0.08f, 0.10f, 0.95f), HorrorUITheme.ColorBoxBorder, 2f, 14);
                inputImg.type = Image.Type.Sliced;
                inputImg.color = Color.white;
            }

            if (codeInputField.textComponent != null)
            {
                codeInputField.textComponent.font = font;
                codeInputField.textComponent.fontSize = 28;
                codeInputField.textComponent.fontStyle = FontStyle.Bold;
                codeInputField.textComponent.alignment = TextAnchor.MiddleCenter;
                codeInputField.textComponent.color = HorrorUITheme.TextCandleGold;
                HorrorUITheme.AddHorrorTextShadow(codeInputField.textComponent, Color.black, new Vector2(1.5f, -1.5f));
            }

            if (codeInputField.placeholder is Text ph)
            {
                ph.font = font;
                ph.fontSize = 20;
                ph.alignment = TextAnchor.MiddleCenter;
                ph.color = new Color(0.7f, 0.65f, 0.6f, 0.6f);
                ph.text = "Nhập mã số...";
            }
        }

        // 4. Submit Button styling
        if (submitButton != null)
        {
            RectTransform subRt = submitButton.GetComponent<RectTransform>();
            subRt.anchorMin = new Vector2(0.5f, 0f);
            subRt.anchorMax = new Vector2(0.5f, 0f);
            subRt.pivot = new Vector2(0.5f, 0f);
            subRt.sizeDelta = new Vector2(175f, 48f);
            subRt.anchoredPosition = new Vector2(-100f, 25f);

            HorrorUITheme.ApplyHorrorButtonStyle(submitButton, HorrorUITheme.ColorBtnResume, HorrorUITheme.ColorBtnResumeBorder);

            Text btnTxt = submitButton.GetComponentInChildren<Text>();
            if (btnTxt != null)
            {
                btnTxt.font = font;
                btnTxt.text = "MỞ KHÓA";
                btnTxt.fontSize = 20;
                btnTxt.fontStyle = FontStyle.Bold;
                btnTxt.color = HorrorUITheme.TextBoneWhite;
                HorrorUITheme.AddHorrorTextShadow(btnTxt, Color.black, new Vector2(1.5f, -1.5f));
            }
        }

        // 5. Cancel Button styling
        if (cancelButton != null)
        {
            RectTransform canRt = cancelButton.GetComponent<RectTransform>();
            canRt.anchorMin = new Vector2(0.5f, 0f);
            canRt.anchorMax = new Vector2(0.5f, 0f);
            canRt.pivot = new Vector2(0.5f, 0f);
            canRt.sizeDelta = new Vector2(175f, 48f);
            canRt.anchoredPosition = new Vector2(100f, 25f);

            HorrorUITheme.ApplyHorrorButtonStyle(cancelButton, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);

            Text btnTxt = cancelButton.GetComponentInChildren<Text>();
            if (btnTxt != null)
            {
                btnTxt.font = font;
                btnTxt.text = "ĐÓNG";
                btnTxt.fontSize = 20;
                btnTxt.fontStyle = FontStyle.Bold;
                btnTxt.color = HorrorUITheme.TextBoneWhite;
                HorrorUITheme.AddHorrorTextShadow(btnTxt, Color.black, new Vector2(1.5f, -1.5f));
            }
        }

        // 6. Feedback Text
        if (feedbackText != null)
        {
            RectTransform fbRt = feedbackText.GetComponent<RectTransform>();
            fbRt.anchorMin = new Vector2(0.5f, 0.5f);
            fbRt.anchorMax = new Vector2(0.5f, 0.5f);
            fbRt.pivot = new Vector2(0.5f, 0.5f);
            fbRt.sizeDelta = new Vector2(440f, 35f);
            fbRt.anchoredPosition = new Vector2(0f, -36f);

            feedbackText.font = font;
            feedbackText.fontSize = 17;
            feedbackText.fontStyle = FontStyle.Bold;
            feedbackText.alignment = TextAnchor.MiddleCenter;
            feedbackText.color = HorrorUITheme.TextBloodRed;
            HorrorUITheme.AddHorrorTextShadow(feedbackText, Color.black, new Vector2(1.5f, -1.5f));
        }
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
            Cancel();
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
        instance.ApplyHorrorTheme();
        
        if (instance.panel != null)
        {
            instance.panel.SetActive(true);
            instance.panel.transform.SetAsLastSibling();

            if (instance.titleText != null && lockable != null)
            {
                instance.titleText.text = lockable.GetDisplayName();
            }
            
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
            
            // Pause game
            Time.timeScale = 0f;
            
            // Unlock cursor for typing
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Hide HUD reticle while typing
            MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
            if (hud != null) hud.SetReticleVisible(false);
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
            ShowFeedback("Vui lòng nhập mật mã!", HorrorUITheme.TextBloodRed);
            return;
        }
        
        // Try unlock
        bool success = currentLockable.TryUnlockWithCode(inputCode);
        
        if (success)
        {
            ShowFeedback("✓ MỞ KHÓA THÀNH CÔNG! PHONG ẤN ĐÃ BỊ PHÁ BỎ.", HorrorUITheme.ColorBtnResumeBorder);
            Invoke(nameof(Hide), 0.75f);
        }
        else
        {
            ShowFeedback("❌ SAI MẬT MÃ! TIẾNG CƯỜI KỲ DỊ VANG LÊN...", HorrorUITheme.TextBloodRed);
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        }
    }
    
    public void Cancel()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        Hide();
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
        }
    }
    
    void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        
        currentLockable = null;
        
        // Resume game
        Time.timeScale = 1f;
        
        // Return cursor state
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (hud != null) hud.SetReticleVisible(true);
    }
}
