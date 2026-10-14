using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the in-game Mirror House HUD without requiring external UI artwork.
/// The HUD is cosmetic only: it never blocks input and works with any Canvas.
/// </summary>
public class MirrorHouseHorrorHUD : MonoBehaviour
{
    private const string HudRootName = "MirrorHouse_HorrorHUD";

    [Header("Theme")]
    [SerializeField] private Color bloodRed = new Color(0.72f, 0.04f, 0.07f, 1f);
    [SerializeField] private Color candleGold = new Color(0.92f, 0.66f, 0.25f, 1f);
    [SerializeField] private Color mistColor = new Color(0.25f, 0.37f, 0.42f, 0.08f);
    [SerializeField, Range(0f, 1f)] private float vignetteStrength = 0.7f;

    private Image vignette;
    private Image mistLeft;
    private Image mistRight;
    private Text promptText;
    private Text titleText;
    private RectTransform reticle;
    private Image eyeOutline;
    private Image eyePupil;
    private float targetVisible;

    // Horror Clue & Mirror Reflection Message Box
    private GameObject cluePanel;
    private Text clueContentText;
    private float clueTimer;

    public static MirrorHouseHorrorHUD EnsureExists()
    {
        MirrorHouseHorrorHUD existing = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        if (existing != null)
        {
            return existing;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        GameObject root = new GameObject(HudRootName, typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        return root.AddComponent<MirrorHouseHorrorHUD>();
    }

    private void Awake()
    {
        BuildIfNeeded();
    }

    private void OnEnable()
    {
        GameEvents.OnCluePickup += HandleClueReceived;
    }

    private void OnDisable()
    {
        GameEvents.OnCluePickup -= HandleClueReceived;
    }

    private void HandleClueReceived(string clueId, string clueText)
    {
        ShowClueMessage(clueText);
    }

    public void ShowClueMessage(string message, float duration = 7.5f)
    {
        if (cluePanel != null && clueContentText != null)
        {
            clueContentText.text = message;
            cluePanel.SetActive(true);
            clueTimer = duration;
        }
    }

    public void CloseClueMessage()
    {
        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
            clueTimer = 0f;
        }
    }

    private void Update()
    {
        if (mistLeft != null && mistRight != null)
        {
            float drift = Mathf.Sin(Time.unscaledTime * 0.23f) * 34f;
            mistLeft.rectTransform.anchoredPosition = new Vector2(drift, -16f);
            mistRight.rectTransform.anchoredPosition = new Vector2(-drift, 18f);
        }

        if (reticle != null)
        {
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * 4.5f) * 0.035f * targetVisible;
            reticle.localScale = Vector3.one * pulse;
        }

        if (promptText != null)
        {
            float alpha = Mathf.MoveTowards(promptText.color.a, targetVisible, Time.unscaledDeltaTime * 7f);
            promptText.color = new Color(candleGold.r, candleGold.g, candleGold.b, alpha);
        }

        if (clueTimer > 0f)
        {
            clueTimer -= Time.deltaTime;
            if (clueTimer <= 0f && cluePanel != null)
            {
                cluePanel.SetActive(false);
            }
        }
    }

    public void SetInteractionTarget(Interactable target)
    {
        targetVisible = target == null ? 0f : 1f;
        if (target == null || promptText == null)
        {
            return;
        }

        string prompt = target.InteractionPrompt;
        promptText.text = string.IsNullOrEmpty(prompt) ? "[ E ]  TƯƠNG TÁC" : prompt;
    }

    public void SetCrosshairHighlighted(bool highlighted)
    {
        if (eyeOutline == null || eyePupil == null)
        {
            return;
        }

        eyeOutline.color = highlighted
            ? new Color(bloodRed.r, bloodRed.g, bloodRed.b, 1f)
            : new Color(0.78f, 0.74f, 0.65f, 0.82f);
        eyePupil.color = highlighted
            ? new Color(1f, 0.08f, 0.1f, 1f)
            : new Color(bloodRed.r, bloodRed.g, bloodRed.b, 0.9f);
    }

    public void SetReticleVisible(bool visible)
    {
        if (reticle != null)
        {
            reticle.gameObject.SetActive(visible);
        }
    }

    private void BuildIfNeeded()
    {
        if (vignette != null)
        {
            return;
        }

        RectTransform root = transform as RectTransform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        vignette = CreateImage("Cracked_Mirror_Vignette", transform, CreateVignetteSprite(), Color.white);
        Stretch(vignette.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        vignette.color = new Color(0.13f, 0.005f, 0.015f, vignetteStrength);

        mistLeft = CreateImage("Cold_Mist_Left", transform, CreateSoftSprite(), mistColor);
        Stretch(mistLeft.rectTransform, new Vector2(-0.15f, -0.08f), new Vector2(0.65f, 0.34f), Vector2.zero, Vector2.zero);
        mistLeft.raycastTarget = false;

        mistRight = CreateImage("Cold_Mist_Right", transform, CreateSoftSprite(), mistColor);
        Stretch(mistRight.rectTransform, new Vector2(0.35f, 0.65f), new Vector2(1.15f, 1.08f), Vector2.zero, Vector2.zero);
        mistRight.raycastTarget = false;

        CreateTitle();
        CreateReticle();
        CreatePrompt();
        CreateCluePanel();
    }

    private void CreateCluePanel()
    {
        cluePanel = new GameObject("Horror_Clue_Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        cluePanel.transform.SetParent(transform, false);

        Image bgImage = cluePanel.GetComponent<Image>();
        bgImage.sprite = HorrorUITheme.GetRoundedBoxSprite(HorrorUITheme.ColorCryptDark, HorrorUITheme.ColorBoxBorderGold, 3f, 16);
        bgImage.type = Image.Type.Sliced;
        bgImage.raycastTarget = true;

        RectTransform rect = cluePanel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0.22f);
        rect.anchorMax = new Vector2(0.8f, 0.78f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Title
        Text headerText = CreateText("Panel_Header", cluePanel.transform, "📜 MANH MỐI / THÔNG ĐIỆP CÕI GƯƠNG", 18, TextAnchor.MiddleCenter);
        headerText.color = HorrorUITheme.TextCandleGold;
        headerText.fontStyle = FontStyle.Bold;
        HorrorUITheme.AddHorrorTextShadow(headerText, Color.black, new Vector2(1.5f, -1.5f));
        Stretch(headerText.rectTransform, new Vector2(0.05f, 0.86f), new Vector2(0.95f, 0.97f), Vector2.zero, Vector2.zero);

        // Content
        clueContentText = CreateText("Panel_Content", cluePanel.transform, "", 16, TextAnchor.UpperLeft);
        clueContentText.color = HorrorUITheme.TextBoneWhite;
        clueContentText.lineSpacing = 1.25f;
        clueContentText.horizontalOverflow = HorizontalWrapMode.Wrap;
        clueContentText.verticalOverflow = VerticalWrapMode.Truncate;
        Stretch(clueContentText.rectTransform, new Vector2(0.06f, 0.16f), new Vector2(0.94f, 0.84f), Vector2.zero, Vector2.zero);

        // Close Hint
        Text closeHint = CreateText("Close_Hint", cluePanel.transform, "[ Chạm vào đây hoặc bấm E để đóng ]", 14, TextAnchor.MiddleCenter);
        closeHint.color = new Color(0.7f, 0.65f, 0.6f, 0.85f);
        Stretch(closeHint.rectTransform, new Vector2(0.1f, 0.03f), new Vector2(0.9f, 0.13f), Vector2.zero, Vector2.zero);

        // Click to close button
        Button btn = cluePanel.AddComponent<Button>();
        btn.onClick.AddListener(CloseClueMessage);

        cluePanel.SetActive(false);
    }

    private void CreateTitle()
    {
        titleText = CreateText("Whisper_Title", transform, "MIRROR HOUSE", 20, TextAnchor.UpperCenter);
        titleText.color = new Color(candleGold.r, candleGold.g, candleGold.b, 0.72f);
        titleText.fontStyle = FontStyle.Bold;
        HorrorUITheme.AddHorrorTextShadow(titleText, Color.black, new Vector2(1.5f, -1.5f));
        Stretch(titleText.rectTransform, new Vector2(0.25f, 0.93f), new Vector2(0.75f, 0.98f), Vector2.zero, Vector2.zero);
    }

    private void CreateReticle()
    {
        GameObject root = new GameObject("Watchful_Eye_Reticle", typeof(RectTransform));
        root.transform.SetParent(transform, false);
        reticle = root.GetComponent<RectTransform>();
        reticle.anchorMin = new Vector2(0.5f, 0.5f);
        reticle.anchorMax = new Vector2(0.5f, 0.5f);
        reticle.sizeDelta = new Vector2(42f, 30f);

        eyeOutline = CreateImage("Eye_Outline", root.transform, CreateRingSprite(), new Color(0.78f, 0.74f, 0.65f, 0.82f));
        Stretch(eyeOutline.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        eyePupil = CreateImage("Eye_Pupil", root.transform, CreateSoftSprite(), new Color(bloodRed.r, bloodRed.g, bloodRed.b, 0.9f));
        eyePupil.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        eyePupil.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        eyePupil.rectTransform.sizeDelta = new Vector2(8f, 8f);
    }

    private void CreatePrompt()
    {
        promptText = CreateText("Reflection_Prompt", transform, "", 18, TextAnchor.MiddleCenter);
        promptText.fontStyle = FontStyle.Bold;
        promptText.color = new Color(candleGold.r, candleGold.g, candleGold.b, 0f);
        HorrorUITheme.AddHorrorTextShadow(promptText, Color.black, new Vector2(1.5f, -1.5f));
        Stretch(promptText.rectTransform, new Vector2(0.2f, 0.36f), new Vector2(0.8f, 0.42f), Vector2.zero, Vector2.zero);
    }

    private static Image CreateImage(string objectName, Transform parent, Sprite sprite, Color color)
    {
        GameObject child = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        child.transform.SetParent(parent, false);
        Image image = child.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static Text CreateText(string objectName, Transform parent, string value, int size, TextAnchor alignment)
    {
        GameObject child = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        child.transform.SetParent(parent, false);
        Text text = child.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = size;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static void Stretch(RectTransform rect, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static Sprite CreateVignetteSprite()
    {
        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x / (float)(size - 1) - 0.5f) * 2f;
                float dy = (y / (float)(size - 1) - 0.5f) * 2f;
                float distance = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
                float alpha = Mathf.SmoothStep(0f, 1f, (distance - 0.35f) / 0.65f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private static Sprite CreateSoftSprite()
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y) / (size - 1f), Vector2.one * 0.5f) * 2f;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(1f - distance)));
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private static Sprite CreateRingSprite()
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x, y) / (size - 1f) - Vector2.one * 0.5f;
                float ellipse = Mathf.Sqrt(point.x * point.x + point.y * point.y * 2.2f);
                float alpha = ellipse > 0.35f && ellipse < 0.47f ? 1f : 0f;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}