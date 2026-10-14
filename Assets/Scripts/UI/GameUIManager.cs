using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GAME UI MANAGER - Quản lý toàn bộ giao diện trong game:
/// HUD, Inventory, Pause Menu, Settings Menu, Clue Viewer, Game Over, Victory.
/// Có khả năng tự động tạo UI chuẩn tại runtime nếu scene chưa gắn sẵn Prefab.
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject cluePanel;

    [Header("HUD Elements")]
    [SerializeField] private Text inventoryText;
    [SerializeField] private Text victoryTimeText;
    [SerializeField] private Text clueContentText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button sprintButton;

    [Header("Settings Controls")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text sensitivityValueText;

    private readonly List<string> collectedKeys = new List<string>();
    private bool isSprintingToggled = false;
    private PlayerController playerController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        EnsureUIComponents();
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        // Setup settings sliders with saved values
        SetupSettingsSliders();

        // Hide modal panels
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (cluePanel != null) cluePanel.SetActive(false);

        UpdateInventoryUI();
    }

    private void OnEnable()
    {
        GameEvents.OnKeyPickup += HandleKeyPickup;
        GameEvents.OnCluePickup += HandleCluePickup;
    }

    private void OnDisable()
    {
        GameEvents.OnKeyPickup -= HandleKeyPickup;
        GameEvents.OnCluePickup -= HandleCluePickup;
    }

    // ================= Event Handlers =================
    private void HandleKeyPickup(string keyId)
    {
        if (!collectedKeys.Contains(keyId))
        {
            collectedKeys.Add(keyId);
            UpdateInventoryUI();
        }
    }

    private void HandleCluePickup(string clueId, string text)
    {
        ShowClue(text);
    }

    private void UpdateInventoryUI()
    {
        if (inventoryText == null) return;

        if (collectedKeys.Count == 0)
        {
            inventoryText.text = "Túi đồ: (Trống)";
        }
        else
        {
            inventoryText.text = "Túi đồ: " + string.Join(", ", collectedKeys.ConvertAll(k => $"[🗝️ {k}]"));
        }
    }

    // ================= Panel Controls =================
    public void ShowPauseMenu(bool show)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(show);
            if (show) pausePanel.transform.SetAsLastSibling();
        }

        MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (hud != null) hud.SetReticleVisible(!show);
    }

    public void ShowSettingsMenu(bool show)
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(show);
            if (show) settingsPanel.transform.SetAsLastSibling();
        }

        MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (hud != null) hud.SetReticleVisible(!show);
    }

    public void ShowGameOver(bool show)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(show);
            if (show) gameOverPanel.transform.SetAsLastSibling();
        }

        MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (hud != null) hud.SetReticleVisible(!show);
    }

    public void ShowVictory(bool show, float timeSeconds = 0f)
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(show);
            if (show) victoryPanel.transform.SetAsLastSibling();
            if (victoryTimeText != null)
            {
                int minutes = Mathf.FloorToInt(timeSeconds / 60f);
                int seconds = Mathf.FloorToInt(timeSeconds % 60f);
                victoryTimeText.text = $"Thời gian trốn thoát: {minutes:00}:{seconds:00}";
            }
        }

        MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (hud != null) hud.SetReticleVisible(!show);
    }

    private void Update()
    {
        if (cluePanel != null && cluePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E) ||
                Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                CloseClue();
            }
        }
    }

    public void ShowClue(string text)
    {
        if (cluePanel != null)
        {
            if (clueContentText != null)
                clueContentText.text = text;
            cluePanel.SetActive(true);
            cluePanel.transform.SetAsLastSibling();
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (hud != null) hud.SetReticleVisible(false);
    }

    public void CloseClue()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);

        Time.timeScale = 1f;

        MirrorHouseHorrorHUD hud = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (hud != null) hud.SetReticleVisible(true);
    }

    // ================= Button Click Handlers =================
    public void OnResumeClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        if (MirrorHouseGameManager.Instance != null)
            MirrorHouseGameManager.Instance.ResumeGame();
        else
            ShowPauseMenu(false);
    }

    public void OnSettingsClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        ShowSettingsMenu(true);
    }

    public void OnCloseSettingsClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        ShowSettingsMenu(false);
    }

    public void OnRestartClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        if (MirrorHouseGameManager.Instance != null)
            MirrorHouseGameManager.Instance.RestartLevel();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        if (MirrorHouseGameManager.Instance != null)
            MirrorHouseGameManager.Instance.LoadMainMenu();
    }

    public void OnQuitClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        if (MirrorHouseGameManager.Instance != null)
            MirrorHouseGameManager.Instance.QuitGame();
        else
        {
            Debug.Log("[GameUIManager] Thoát game...");
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    public void OnSprintToggleClicked()
    {
        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();

        if (playerController != null)
        {
            isSprintingToggled = !isSprintingToggled;
            playerController.SetSprinting(isSprintingToggled);

            if (sprintButton != null)
            {
                Image btnImg = sprintButton.GetComponent<Image>();
                if (btnImg != null)
                {
                    Color sprintFill = isSprintingToggled ? new Color(0.35f, 0.10f, 0.12f, 0.95f) : new Color(0.12f, 0.08f, 0.10f, 0.8f);
                    Color sprintBorder = isSprintingToggled ? HorrorUITheme.ColorBoxBorder : HorrorUITheme.ColorBoxBorderGold;
                    btnImg.sprite = HorrorUITheme.GetRoundedBoxSprite(sprintFill, sprintBorder, 2.5f, 16);
                }
            }
        }
    }

    // ================= Settings Sliders =================
    private void SetupSettingsSliders()
    {
        PlayerCamera cam = FindAnyObjectByType<PlayerCamera>();
        float curSens = cam != null ? cam.GetSensitivity() : PlayerPrefs.GetFloat("CameraSensitivity", 1.2f);
        if (curSens > 3.5f) curSens = 1.2f;

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.2f;
            sensitivitySlider.maxValue = 3.5f;
            sensitivitySlider.value = curSens;
            sensitivitySlider.onValueChanged.AddListener(val =>
            {
                if (cam != null) cam.SetSensitivity(val);
                if (sensitivityValueText != null) sensitivityValueText.text = $"{val:F1}x";
            });
            if (sensitivityValueText != null) sensitivityValueText.text = $"{curSens:F1}x";
        }

        if (AudioManager.Instance != null)
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
                masterVolumeSlider.onValueChanged.AddListener(val => AudioManager.Instance.SetMasterVolume(val));
            }
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
                musicVolumeSlider.onValueChanged.AddListener(val => AudioManager.Instance.SetMusicVolume(val));
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
                sfxVolumeSlider.onValueChanged.AddListener(val => AudioManager.Instance.SetSFXVolume(val));
            }
        }
    }

    // ================= Procedural UI Construction =================
    private void EnsureUIComponents()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas_Auto");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            transform.SetParent(canvas.transform, false);
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 1. Inventory HUD Text
        if (inventoryText == null)
        {
            Transform existingBadge = canvas.transform.Find("HUD_InventoryBadge");
            if (existingBadge != null) DestroyImmediate(existingBadge.gameObject);

            GameObject badgeObj = new GameObject("HUD_InventoryBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeObj.transform.SetParent(canvas.transform, false);
            RectTransform brt = badgeObj.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0f, 1f);
            brt.anchorMax = new Vector2(0f, 1f);
            brt.pivot = new Vector2(0f, 1f);
            brt.anchoredPosition = new Vector2(60f, -50f);
            brt.sizeDelta = new Vector2(500f, 54f);

            Image badgeImg = badgeObj.GetComponent<Image>();
            badgeImg.sprite = HorrorUITheme.GetRoundedBoxSprite(new Color(0.08f, 0.05f, 0.06f, 0.85f), HorrorUITheme.ColorBoxBorderGold, 2f, 14);
            badgeImg.type = Image.Type.Sliced;
            badgeImg.color = Color.white;

            GameObject invObj = new GameObject("HUD_InventoryText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            invObj.transform.SetParent(badgeObj.transform, false);
            RectTransform rt = invObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(16f, 0f);
            rt.offsetMax = new Vector2(-16f, 0f);

            inventoryText = invObj.GetComponent<Text>();
            inventoryText.font = font;
            inventoryText.fontSize = 22;
            inventoryText.alignment = TextAnchor.MiddleLeft;
            inventoryText.color = HorrorUITheme.TextCandleGold;
            inventoryText.text = "Túi đồ: (Trống)";
            inventoryText.raycastTarget = false;
            HorrorUITheme.AddHorrorTextShadow(inventoryText, Color.black, new Vector2(1.5f, -1.5f));
        }

        // 2. Pause Button (Top Right - an toàn khỏi viền cong và camera notch)
        if (pauseButton == null)
        {
            Transform existingPb = canvas.transform.Find("HUD_PauseButton");
            if (existingPb != null) DestroyImmediate(existingPb.gameObject);

            GameObject pbObj = new GameObject("HUD_PauseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            pbObj.transform.SetParent(canvas.transform, false);
            RectTransform rt = pbObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-60f, -50f);
            rt.sizeDelta = new Vector2(70f, 70f);

            pauseButton = pbObj.GetComponent<Button>();
            pauseButton.onClick.AddListener(() =>
            {
                if (MirrorHouseGameManager.Instance != null)
                    MirrorHouseGameManager.Instance.PauseGame();
                else
                    ShowPauseMenu(true);
            });
            HorrorUITheme.ApplyHorrorButtonStyle(pauseButton, new Color(0.12f, 0.08f, 0.10f, 0.85f), HorrorUITheme.ColorBoxBorderGold);

            GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            txtObj.transform.SetParent(pbObj.transform, false);
            Text t = txtObj.GetComponent<Text>();
            t.font = font;
            t.text = "❚❚";
            t.fontSize = 28;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = HorrorUITheme.TextBoneWhite;
            t.raycastTarget = false;
            HorrorUITheme.AddHorrorTextShadow(t, Color.black, new Vector2(1.5f, -1.5f));
            Stretch(txtObj.GetComponent<RectTransform>());
        }

        // 3. Sprint Button (Bottom Right, cân đối ngay phía trên nút E)
        if (sprintButton == null)
        {
            Transform existingSb = canvas.transform.Find("HUD_SprintButton");
            GameObject sbObj = null;
            if (existingSb != null)
            {
                sbObj = existingSb.gameObject;
            }
            else
            {
                sbObj = new GameObject("HUD_SprintButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                sbObj.transform.SetParent(canvas.transform, false);
            }

            RectTransform rt = sbObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(-190f, 320f);
            rt.sizeDelta = new Vector2(100f, 80f);

            sprintButton = sbObj.GetComponent<Button>();
            sprintButton.onClick.RemoveAllListeners();
            sprintButton.onClick.AddListener(OnSprintToggleClicked);
            HorrorUITheme.ApplyHorrorButtonStyle(sprintButton, new Color(0.12f, 0.08f, 0.10f, 0.8f), HorrorUITheme.ColorBoxBorder);

            Text t = sbObj.GetComponentInChildren<Text>();
            if (t == null)
            {
                GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
                txtObj.transform.SetParent(sbObj.transform, false);
                t = txtObj.GetComponent<Text>();
                Stretch(txtObj.GetComponent<RectTransform>());
            }
            if (font != null) t.font = font;
            t.text = "CHẠY";
            t.fontSize = 20;
            t.fontStyle = FontStyle.Bold;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = HorrorUITheme.TextBoneWhite;
            t.raycastTarget = false;
            HorrorUITheme.AddHorrorTextShadow(t, Color.black, new Vector2(1.5f, -1.5f));
        }

        // 4. Pause Menu Panel
        if (pausePanel == null)
        {
            Transform existing = canvas.transform.Find("Panel_PauseMenu");
            if (existing != null) DestroyImmediate(existing.gameObject);

            pausePanel = CreateMenuPanel("Panel_PauseMenu", canvas.transform, "TẠM DỪNG", font, out Transform contentRoot);
            CreateButton(contentRoot, "Tiếp tục", font, OnResumeClicked, HorrorUITheme.ColorBtnResume, HorrorUITheme.ColorBtnResumeBorder);
            CreateButton(contentRoot, "Cài đặt", font, OnSettingsClicked, HorrorUITheme.ColorBtnSettings, HorrorUITheme.ColorBtnSettingsBorder);
            CreateButton(contentRoot, "Chơi lại", font, OnRestartClicked, HorrorUITheme.ColorBtnRestart, HorrorUITheme.ColorBtnRestartBorder);
            CreateButton(contentRoot, "Menu chính", font, OnMainMenuClicked, HorrorUITheme.ColorBtnMenu, HorrorUITheme.ColorBtnMenuBorder);
            CreateButton(contentRoot, "Thoát game", font, OnQuitClicked, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);
        }

        // 5. Settings Menu Panel
        if (settingsPanel == null)
        {
            Transform existing = canvas.transform.Find("Panel_SettingsMenu");
            if (existing != null) DestroyImmediate(existing.gameObject);

            settingsPanel = CreateMenuPanel("Panel_SettingsMenu", canvas.transform, "CÀI ĐẶT", font, out Transform contentRoot);
            sensitivitySlider = CreateSliderRow(contentRoot, "Độ nhạy camera", font, out sensitivityValueText);
            masterVolumeSlider = CreateSliderRow(contentRoot, "Âm lượng tổng (Master)", font, out _);
            musicVolumeSlider = CreateSliderRow(contentRoot, "Nhạc nền (BGM)", font, out _);
            sfxVolumeSlider = CreateSliderRow(contentRoot, "Hiệu ứng (SFX)", font, out _);
            CreateButton(contentRoot, "Đóng", font, OnCloseSettingsClicked, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);
        }

        // 6. Game Over Panel
        if (gameOverPanel == null)
        {
            Transform existing = canvas.transform.Find("Panel_GameOver");
            if (existing != null) DestroyImmediate(existing.gameObject);

            gameOverPanel = CreateModalPanel("Panel_GameOver", canvas.transform, font,
                "BẠN ĐÃ BỊ BẮT!",
                "Thực thể trong gương đã tóm được bạn...",
                HorrorUITheme.TextBloodRed,
                out Transform contentRoot);
            CreateButton(contentRoot, "Chơi lại", font, OnRestartClicked, HorrorUITheme.ColorBtnRestart, HorrorUITheme.ColorBtnRestartBorder);
            CreateButton(contentRoot, "Menu chính", font, OnMainMenuClicked, HorrorUITheme.ColorBtnMenu, HorrorUITheme.ColorBtnMenuBorder);
            CreateButton(contentRoot, "Thoát game", font, OnQuitClicked, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);
        }

        // 7. Victory Panel
        if (victoryPanel == null)
        {
            Transform existing = canvas.transform.Find("Panel_Victory");
            if (existing != null) DestroyImmediate(existing.gameObject);

            victoryPanel = CreateModalPanel("Panel_Victory", canvas.transform, font,
                "THOÁT THÀNH CÔNG!",
                "Bạn đã phá vỡ lời nguyền và trốn thoát!",
                HorrorUITheme.TextCandleGold,
                out Transform contentRoot);

            GameObject timeObj = new GameObject("TimeText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
            timeObj.transform.SetParent(contentRoot, false);
            victoryTimeText = timeObj.GetComponent<Text>();
            victoryTimeText.font = font;
            victoryTimeText.fontSize = 24;
            victoryTimeText.alignment = TextAnchor.MiddleCenter;
            victoryTimeText.color = HorrorUITheme.TextCandleGold;
            HorrorUITheme.AddHorrorTextShadow(victoryTimeText, Color.black, new Vector2(1f, -1f));
            LayoutElement timeLe = timeObj.GetComponent<LayoutElement>();
            timeLe.preferredWidth = 440f;
            timeLe.preferredHeight = 40f;
            timeLe.minHeight = 35f;

            CreateButton(contentRoot, "Chơi lại", font, OnRestartClicked, HorrorUITheme.ColorBtnResume, HorrorUITheme.ColorBtnResumeBorder);
            CreateButton(contentRoot, "Menu chính", font, OnMainMenuClicked, HorrorUITheme.ColorBtnMenu, HorrorUITheme.ColorBtnMenuBorder);
            CreateButton(contentRoot, "Thoát game", font, OnQuitClicked, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);
        }

        // 8. Clue Popup Panel
        if (cluePanel == null)
        {
            Transform existing = canvas.transform.Find("Panel_ClueViewer");
            if (existing != null) DestroyImmediate(existing.gameObject);

            cluePanel = CreateMenuPanel("Panel_ClueViewer", canvas.transform, "MANH MỐI", font, out Transform contentRoot);

            GameObject textObj = new GameObject("ClueContent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
            textObj.transform.SetParent(contentRoot, false);
            clueContentText = textObj.GetComponent<Text>();
            clueContentText.font = font;
            clueContentText.fontSize = 22;
            clueContentText.alignment = TextAnchor.MiddleCenter;
            clueContentText.color = HorrorUITheme.TextBoneWhite;
            HorrorUITheme.AddHorrorTextShadow(clueContentText, Color.black, new Vector2(1f, -1f));
            LayoutElement clueLe = textObj.GetComponent<LayoutElement>();
            clueLe.preferredWidth = 440f;
            clueLe.preferredHeight = 160f;
            clueLe.minHeight = 100f;

            CreateButton(contentRoot, "Đóng manh mối", font, CloseClue, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);
        }

        // 9. Enforce mobile controls layout (Joystick at 190, 190; E at -190, 190; Sprint at -190, 320)
        EnforceMobileControlsLayout();
    }

    private void EnforceMobileControlsLayout()
    {
        // 1. Virtual Joystick bên trái: anchoredPosition = (190, 190), size = 200x200, pivot = (0.5, 0.5)
        GameObject joyObj = GameObject.Find("Fixed Joystick");
        if (joyObj != null)
        {
            RectTransform jrt = joyObj.GetComponent<RectTransform>();
            if (jrt != null)
            {
                jrt.anchorMin = Vector2.zero;
                jrt.anchorMax = Vector2.zero;
                jrt.pivot = new Vector2(0.5f, 0.5f);
                jrt.anchoredPosition = new Vector2(190f, 190f);
                jrt.sizeDelta = new Vector2(200f, 200f);
            }

            Transform handle = joyObj.transform.Find("Handle");
            if (handle != null)
            {
                var handleImg = handle.GetComponent<Image>();
                if (handleImg != null) handleImg.raycastTarget = false;
                RectTransform hrt = handle.GetComponent<RectTransform>();
                if (hrt != null)
                {
                    hrt.anchorMin = new Vector2(0.5f, 0.5f);
                    hrt.anchorMax = new Vector2(0.5f, 0.5f);
                    hrt.pivot = new Vector2(0.5f, 0.5f);
                    hrt.sizeDelta = new Vector2(85f, 85f);
                }
            }
        }

        // 2. Nút Tương Tác E bên phải: anchoredPosition = (-190, 190), size = 120x120, pivot = (0.5, 0.5)
        GameObject mibObj = GameObject.Find("MobileInteractButton");
        if (mibObj != null)
        {
            RectTransform mrt = mibObj.GetComponent<RectTransform>();
            if (mrt != null)
            {
                mrt.anchorMin = new Vector2(1f, 0f);
                mrt.anchorMax = new Vector2(1f, 0f);
                mrt.pivot = new Vector2(0.5f, 0.5f);
                mrt.anchoredPosition = new Vector2(-190f, 190f);
                mrt.sizeDelta = new Vector2(120f, 120f);
            }
        }
    }

    // ================= Helper Creation Methods =================
    private static GameObject CreateMenuPanel(string name, Transform parent, string title, Font font, out Transform content)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);
        Stretch(panel.GetComponent<RectTransform>());
        panel.GetComponent<Image>().color = new Color(0.02f, 0.015f, 0.02f, 0.94f);

        GameObject box = new GameObject("MenuBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        box.transform.SetParent(panel.transform, false);
        RectTransform rt = box.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(500f, 540f);

        Image boxImg = box.GetComponent<Image>();
        boxImg.sprite = HorrorUITheme.GetRoundedBoxSprite(HorrorUITheme.ColorCryptDark, HorrorUITheme.ColorBoxBorderGold, 3f, 22);
        boxImg.type = Image.Type.Sliced;
        boxImg.color = Color.white;

        VerticalLayoutGroup vlg = box.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 14f;
        vlg.padding = new RectOffset(35, 35, 28, 28);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = box.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
        titleObj.transform.SetParent(box.transform, false);
        Text t = titleObj.GetComponent<Text>();
        t.font = font;
        t.text = title;
        t.fontSize = 32;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = HorrorUITheme.TextCandleGold;
        HorrorUITheme.AddHorrorTextOutline(t, new Color(0.12f, 0.01f, 0.02f, 0.95f), new Vector2(2f, -2f));
        HorrorUITheme.AddHorrorTextShadow(t, new Color(0.35f, 0.1f, 0.1f, 0.6f), new Vector2(3f, -3f));

        LayoutElement titleLe = titleObj.GetComponent<LayoutElement>();
        titleLe.preferredWidth = 430f;
        titleLe.preferredHeight = 50f;
        titleLe.minHeight = 45f;

        content = box.transform;
        return panel;
    }

    private static GameObject CreateModalPanel(string name, Transform parent, Font font, string title, string subtitle, Color titleColor, out Transform content)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);
        Stretch(panel.GetComponent<RectTransform>());
        panel.GetComponent<Image>().color = new Color(0.02f, 0.01f, 0.015f, 0.94f);

        GameObject box = new GameObject("ModalBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        box.transform.SetParent(panel.transform, false);
        RectTransform rt = box.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(540f, 500f);

        Image boxImg = box.GetComponent<Image>();
        boxImg.sprite = HorrorUITheme.GetRoundedBoxSprite(HorrorUITheme.ColorCryptDark, HorrorUITheme.ColorBoxBorder, 3.5f, 22);
        boxImg.type = Image.Type.Sliced;
        boxImg.color = Color.white;

        VerticalLayoutGroup vlg = box.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 14f;
        vlg.padding = new RectOffset(35, 35, 28, 28);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = box.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
        titleObj.transform.SetParent(box.transform, false);
        Text t = titleObj.GetComponent<Text>();
        t.font = font;
        t.text = title;
        t.fontSize = 36;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = titleColor;
        HorrorUITheme.AddHorrorTextOutline(t, new Color(0.12f, 0.01f, 0.02f, 0.95f), new Vector2(2f, -2f));
        HorrorUITheme.AddHorrorTextShadow(t, new Color(0.45f, 0f, 0f, 0.6f), new Vector2(3f, -3f));

        LayoutElement titleLe = titleObj.GetComponent<LayoutElement>();
        titleLe.preferredWidth = 470f;
        titleLe.preferredHeight = 52f;
        titleLe.minHeight = 45f;

        GameObject subObj = new GameObject("Subtitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
        subObj.transform.SetParent(box.transform, false);
        Text st = subObj.GetComponent<Text>();
        st.font = font;
        st.text = subtitle;
        st.fontSize = 20;
        st.alignment = TextAnchor.MiddleCenter;
        st.color = HorrorUITheme.TextBoneWhite;
        HorrorUITheme.AddHorrorTextShadow(st, Color.black, new Vector2(1f, -1f));

        LayoutElement subLe = subObj.GetComponent<LayoutElement>();
        subLe.preferredWidth = 470f;
        subLe.preferredHeight = 35f;
        subLe.minHeight = 30f;

        content = box.transform;
        return panel;
    }

    private static Button CreateButton(Transform parent, string label, Font font, UnityEngine.Events.UnityAction onClick, Color fill, Color border)
    {
        GameObject btnObj = new GameObject("Btn_" + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(430f, 54f);

        LayoutElement le = btnObj.GetComponent<LayoutElement>();
        le.preferredWidth = 430f;
        le.preferredHeight = 54f;
        le.minHeight = 50f;
        le.flexibleWidth = 1f;
        le.flexibleHeight = 0f;

        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(onClick);
        HorrorUITheme.ApplyHorrorButtonStyle(btn, fill, border);

        GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        txtObj.transform.SetParent(btnObj.transform, false);
        Text t = txtObj.GetComponent<Text>();
        t.font = font;
        t.text = label;
        t.fontSize = 22;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = HorrorUITheme.TextBoneWhite;
        t.raycastTarget = false;
        HorrorUITheme.AddHorrorTextShadow(t, Color.black, new Vector2(1.5f, -1.5f));
        Stretch(txtObj.GetComponent<RectTransform>());

        return btn;
    }

    private static Slider CreateSliderRow(Transform parent, string label, Font font, out Text valText)
    {
        GameObject row = new GameObject("Row_" + label, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.transform.SetParent(parent, false);
        RectTransform rt = row.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(430f, 42f);

        LayoutElement rowLe = row.GetComponent<LayoutElement>();
        rowLe.preferredWidth = 430f;
        rowLe.preferredHeight = 42f;
        rowLe.minHeight = 36f;

        HorizontalLayoutGroup hlg = row.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10f;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;

        GameObject lblObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
        lblObj.transform.SetParent(row.transform, false);
        Text lbl = lblObj.GetComponent<Text>();
        lbl.font = font;
        lbl.text = label;
        lbl.fontSize = 18;
        lbl.color = HorrorUITheme.TextCandleGold;
        HorrorUITheme.AddHorrorTextShadow(lbl, Color.black, new Vector2(1f, -1f));
        LayoutElement lblLe = lblObj.GetComponent<LayoutElement>();
        lblLe.preferredWidth = 190f;
        lblLe.preferredHeight = 36f;

        // Slider
        GameObject sliderObj = new GameObject("Slider", typeof(RectTransform), typeof(Slider), typeof(LayoutElement));
        sliderObj.transform.SetParent(row.transform, false);
        LayoutElement sldLe = sliderObj.GetComponent<LayoutElement>();
        sldLe.preferredWidth = 170f;
        sldLe.preferredHeight = 24f;
        Slider slider = sliderObj.GetComponent<Slider>();

        // Background
        GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgObj.transform.SetParent(sliderObj.transform, false);
        Stretch(bgObj.GetComponent<RectTransform>());
        Image bgImg = bgObj.GetComponent<Image>();
        bgImg.sprite = HorrorUITheme.GetRoundedBoxSprite(new Color(0.12f, 0.08f, 0.1f, 0.95f), new Color(0.4f, 0.2f, 0.25f, 0.8f), 1.5f, 10);
        bgImg.type = Image.Type.Sliced;
        bgImg.color = Color.white;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        Stretch(fillArea.GetComponent<RectTransform>());

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        Stretch(fill.GetComponent<RectTransform>());
        Image fillImg = fill.GetComponent<Image>();
        fillImg.sprite = HorrorUITheme.GetRoundedBoxSprite(new Color(0.85f, 0.15f, 0.22f, 1f), new Color(1f, 0.35f, 0.4f, 1f), 1f, 8);
        fillImg.type = Image.Type.Sliced;
        fillImg.color = Color.white;
        slider.fillRect = fill.GetComponent<RectTransform>();

        // Value text
        GameObject valObj = new GameObject("ValText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
        valObj.transform.SetParent(row.transform, false);
        valText = valObj.GetComponent<Text>();
        valText.font = font;
        valText.text = "1.0";
        valText.fontSize = 16;
        valText.alignment = TextAnchor.MiddleLeft;
        valText.color = HorrorUITheme.TextBoneWhite;
        LayoutElement valLe = valObj.GetComponent<LayoutElement>();
        valLe.preferredWidth = 50f;
        valLe.preferredHeight = 36f;

        return slider;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
