using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// MAIN MENU UI - Giao diện màn hình chính của game MirrorHouse
/// Cung cấp các nút: Bắt đầu chơi, Cài đặt, Thoát game.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Level_Main";

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Settings Controls")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text sensitivityValueText;

    private void Awake()
    {
        EnsureMainMenuUI();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        SetupSettings();
    }

    public void OnStartGameClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOpenSettingsClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OnCloseSettingsClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OnQuitClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        Debug.Log("[MainMenuUI] Thoát game...");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void SetupSettings()
    {
        float curSens = PlayerPrefs.GetFloat("CameraSensitivity", 1.2f);
        if (curSens > 3.5f) curSens = 1.2f;
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.2f;
            sensitivitySlider.maxValue = 3.5f;
            sensitivitySlider.value = curSens;
            sensitivitySlider.onValueChanged.AddListener(val =>
            {
                PlayerPrefs.SetFloat("CameraSensitivity", val);
                PlayerPrefs.Save();
                if (sensitivityValueText != null) sensitivityValueText.text = $"{val:F1}x";
            });
            if (sensitivityValueText != null) sensitivityValueText.text = $"{curSens:F1}x";
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.onValueChanged.AddListener(val =>
            {
                PlayerPrefs.SetFloat("MasterVolume", val);
                PlayerPrefs.Save();
                if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(val);
            });
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
            musicVolumeSlider.onValueChanged.AddListener(val =>
            {
                PlayerPrefs.SetFloat("MusicVolume", val);
                PlayerPrefs.Save();
                if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(val);
            });
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(val =>
            {
                PlayerPrefs.SetFloat("SFXVolume", val);
                PlayerPrefs.Save();
                if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(val);
            });
        }
    }

    private void EnsureMainMenuUI()
    {
        // Ensure a Camera exists in the scene so Unity doesn't show "No cameras rendering"
        if (Camera.main == null && FindAnyObjectByType<Camera>() == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.02f, 0.03f, 1f);
            camObj.tag = "MainCamera";
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas_MainMenu");
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

        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        if (mainPanel == null)
        {
            mainPanel = new GameObject("Panel_Main", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            mainPanel.transform.SetParent(canvas.transform, false);
            Stretch(mainPanel.GetComponent<RectTransform>());
            mainPanel.GetComponent<Image>().color = new Color(0.03f, 0.015f, 0.02f, 1f);

            // Container Box with Gothic Rounded Border
            GameObject box = new GameObject("ContentBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup));
            box.transform.SetParent(mainPanel.transform, false);
            RectTransform boxRt = box.GetComponent<RectTransform>();
            boxRt.anchorMin = new Vector2(0.5f, 0.5f);
            boxRt.anchorMax = new Vector2(0.5f, 0.5f);
            boxRt.sizeDelta = new Vector2(600f, 620f);

            Image boxImg = box.GetComponent<Image>();
            boxImg.sprite = HorrorUITheme.GetRoundedBoxSprite(HorrorUITheme.ColorCryptDark, HorrorUITheme.ColorBoxBorder, 3.5f, 24);
            boxImg.type = Image.Type.Sliced;
            boxImg.color = Color.white;

            VerticalLayoutGroup vlg = box.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 20f;
            vlg.padding = new RectOffset(40, 40, 35, 35);
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;

            // Title
            GameObject titleObj = new GameObject("GameTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
            titleObj.transform.SetParent(box.transform, false);
            Text title = titleObj.GetComponent<Text>();
            title.font = font;
            title.text = "MIRROR HOUSE";
            title.fontSize = 52;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = HorrorUITheme.TextBloodRed;
            HorrorUITheme.AddHorrorTextOutline(title, new Color(0.12f, 0.01f, 0.02f, 0.95f), new Vector2(2.5f, -2.5f));
            HorrorUITheme.AddHorrorTextShadow(title, new Color(0.45f, 0f, 0f, 0.5f), new Vector2(4f, -4f));

            LayoutElement titleLe = titleObj.GetComponent<LayoutElement>();
            titleLe.preferredWidth = 520f;
            titleLe.preferredHeight = 70f;

            // Subtitle with Gothic Divider
            GameObject subObj = new GameObject("Subtitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
            subObj.transform.SetParent(box.transform, false);
            Text sub = subObj.GetComponent<Text>();
            sub.font = font;
            sub.text = "── ❖ ──\nTrò chơi kinh dị góc nhìn thứ nhất";
            sub.fontSize = 19;
            sub.alignment = TextAnchor.MiddleCenter;
            sub.color = HorrorUITheme.TextCandleGold;
            HorrorUITheme.AddHorrorTextShadow(sub, Color.black, new Vector2(1f, -1f));

            LayoutElement subLe = subObj.GetComponent<LayoutElement>();
            subLe.preferredWidth = 520f;
            subLe.preferredHeight = 50f;

            // Rounded Gothic Buttons
            CreateBtn(box.transform, "BẮT ĐẦU CHƠI", font, OnStartGameClicked, HorrorUITheme.ColorBtnMenu, HorrorUITheme.ColorBtnMenuBorder);
            CreateBtn(box.transform, "CÀI ĐẶT", font, OnOpenSettingsClicked, HorrorUITheme.ColorBtnSettings, HorrorUITheme.ColorBtnSettingsBorder);
            CreateBtn(box.transform, "THOÁT", font, OnQuitClicked, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);
        }

        if (settingsPanel == null)
        {
            settingsPanel = new GameObject("Panel_Settings", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            settingsPanel.transform.SetParent(canvas.transform, false);
            Stretch(settingsPanel.GetComponent<RectTransform>());
            settingsPanel.GetComponent<Image>().color = new Color(0.02f, 0.01f, 0.02f, 0.95f);

            GameObject box = new GameObject("SettingsBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup));
            box.transform.SetParent(settingsPanel.transform, false);
            RectTransform srt = box.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.5f, 0.5f);
            srt.anchorMax = new Vector2(0.5f, 0.5f);
            srt.sizeDelta = new Vector2(560f, 520f);

            Image srtImg = box.GetComponent<Image>();
            srtImg.sprite = HorrorUITheme.GetRoundedBoxSprite(HorrorUITheme.ColorCryptDark, HorrorUITheme.ColorBoxBorderGold, 3f, 22);
            srtImg.type = Image.Type.Sliced;
            srtImg.color = Color.white;

            VerticalLayoutGroup vlg = box.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 16f;
            vlg.padding = new RectOffset(35, 35, 30, 30);
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
            titleObj.transform.SetParent(box.transform, false);
            Text t = titleObj.GetComponent<Text>();
            t.font = font;
            t.text = "CÀI ĐẶT";
            t.fontSize = 32;
            t.fontStyle = FontStyle.Bold;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = HorrorUITheme.TextCandleGold;
            HorrorUITheme.AddHorrorTextOutline(t, new Color(0.12f, 0.01f, 0.02f, 0.95f), new Vector2(2f, -2f));

            LayoutElement tLe = titleObj.GetComponent<LayoutElement>();
            tLe.preferredWidth = 480f;
            tLe.preferredHeight = 45f;

            sensitivitySlider = CreateSlider(box.transform, "Độ nhạy camera", font, out sensitivityValueText);
            masterVolumeSlider = CreateSlider(box.transform, "Âm lượng tổng (Master)", font, out _);
            musicVolumeSlider = CreateSlider(box.transform, "Nhạc nền (BGM)", font, out _);
            sfxVolumeSlider = CreateSlider(box.transform, "Hiệu ứng (SFX)", font, out _);
            CreateBtn(box.transform, "ĐÓNG", font, OnCloseSettingsClicked, HorrorUITheme.ColorBtnQuit, HorrorUITheme.ColorBtnQuitBorder);
        }
    }

    private static Button CreateBtn(Transform parent, string label, Font font, UnityEngine.Events.UnityAction onClick, Color fill, Color border)
    {
        GameObject btnObj = new GameObject("Btn_" + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400f, 58f);

        LayoutElement le = btnObj.GetComponent<LayoutElement>();
        le.preferredWidth = 400f;
        le.preferredHeight = 58f;
        le.minHeight = 52f;

        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(onClick);
        HorrorUITheme.ApplyHorrorButtonStyle(btn, fill, border);

        GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        txtObj.transform.SetParent(btnObj.transform, false);
        Text t = txtObj.GetComponent<Text>();
        t.font = font;
        t.text = label;
        t.fontSize = 23;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = HorrorUITheme.TextBoneWhite;
        t.raycastTarget = false;
        HorrorUITheme.AddHorrorTextShadow(t, Color.black, new Vector2(1.5f, -1.5f));
        Stretch(txtObj.GetComponent<RectTransform>());

        return btn;
    }

    private static Slider CreateSlider(Transform parent, string label, Font font, out Text valText)
    {
        GameObject row = new GameObject("Row_" + label, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.transform.SetParent(parent, false);
        RectTransform rt = row.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(480f, 42f);

        LayoutElement rowLe = row.GetComponent<LayoutElement>();
        rowLe.preferredWidth = 480f;
        rowLe.preferredHeight = 42f;

        HorizontalLayoutGroup hlg = row.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10f;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        GameObject lblObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        lblObj.transform.SetParent(row.transform, false);
        Text lbl = lblObj.GetComponent<Text>();
        lbl.font = font;
        lbl.text = label;
        lbl.fontSize = 18;
        lbl.color = HorrorUITheme.TextCandleGold;
        lblObj.GetComponent<RectTransform>().sizeDelta = new Vector2(210f, 35f);

        GameObject sliderObj = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
        sliderObj.transform.SetParent(row.transform, false);
        RectTransform srt = sliderObj.GetComponent<RectTransform>();
        srt.sizeDelta = new Vector2(180f, 24f);
        Slider slider = sliderObj.GetComponent<Slider>();

        GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgObj.transform.SetParent(sliderObj.transform, false);
        Stretch(bgObj.GetComponent<RectTransform>());
        Image bgImg = bgObj.GetComponent<Image>();
        bgImg.sprite = HorrorUITheme.GetRoundedBoxSprite(new Color(0.12f, 0.08f, 0.1f, 0.95f), new Color(0.4f, 0.2f, 0.25f, 0.8f), 1.5f, 10);
        bgImg.type = Image.Type.Sliced;
        bgImg.color = Color.white;

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

        GameObject valObj = new GameObject("ValText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        valObj.transform.SetParent(row.transform, false);
        valText = valObj.GetComponent<Text>();
        valText.font = font;
        valText.text = "1.0";
        valText.fontSize = 16;
        valText.alignment = TextAnchor.MiddleLeft;
        valText.color = HorrorUITheme.TextBoneWhite;
        valObj.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 35f);

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
