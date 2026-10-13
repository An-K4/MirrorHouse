using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HORROR UI THEME - Tạo phong cách giao diện kinh dị Gothic sang trọng:
/// - Các ô bo tròn mềm mại (9-sliced procedural rounded rectangles).
/// - Tông màu máu cổ, đá đen obsidian, viền đồng rỉ và ánh sáng u ám.
/// - Hiệu ứng bóng đổ và viền chữ rùng rợn cho Typography.
/// - Tự động tái sử dụng Sprite bộ đệm (Caching) không tốn bộ nhớ.
/// </summary>
public static class HorrorUITheme
{
    private static readonly Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    // Bảng màu Gothic Horror chuẩn
    public static readonly Color ColorCryptDark = new Color(0.06f, 0.04f, 0.05f, 0.94f);      // Đen hầm mộ
    public static readonly Color ColorBoxBorder = new Color(0.48f, 0.12f, 0.16f, 1.0f);       // Viền đỏ máu khô
    public static readonly Color ColorBoxBorderGold = new Color(0.68f, 0.52f, 0.22f, 0.85f);  // Viền vàng cổ

    public static readonly Color ColorBtnResume = new Color(0.12f, 0.28f, 0.14f, 1.0f);       // Xanh rêu tối (Tiếp tục)
    public static readonly Color ColorBtnResumeBorder = new Color(0.25f, 0.65f, 0.30f, 1.0f);
    public static readonly Color ColorBtnSettings = new Color(0.12f, 0.12f, 0.24f, 1.0f);     // Tím u ám (Cài đặt)
    public static readonly Color ColorBtnSettingsBorder = new Color(0.35f, 0.38f, 0.72f, 1.0f);
    public static readonly Color ColorBtnRestart = new Color(0.28f, 0.15f, 0.06f, 1.0f);      // Cam than hồng (Chơi lại)
    public static readonly Color ColorBtnRestartBorder = new Color(0.72f, 0.40f, 0.15f, 1.0f);
    public static readonly Color ColorBtnMenu = new Color(0.28f, 0.08f, 0.10f, 1.0f);         // Đỏ thẫm máu (Menu chính / Bắt đầu)
    public static readonly Color ColorBtnMenuBorder = new Color(0.75f, 0.18f, 0.22f, 1.0f);
    public static readonly Color ColorBtnQuit = new Color(0.12f, 0.10f, 0.12f, 1.0f);         // Xám tro tàn (Thoát)
    public static readonly Color ColorBtnQuitBorder = new Color(0.38f, 0.32f, 0.35f, 1.0f);

    public static readonly Color TextCandleGold = new Color(0.95f, 0.84f, 0.55f, 1.0f);      // Vàng nến ma mị
    public static readonly Color TextBloodRed = new Color(0.85f, 0.12f, 0.16f, 1.0f);        // Đỏ máu tươi
    public static readonly Color TextBoneWhite = new Color(0.92f, 0.90f, 0.86f, 1.0f);       // Trắng tro xương
    public static readonly Color TextMutedAsh = new Color(0.70f, 0.66f, 0.64f, 0.9f);        // Xám khói

    /// <summary>
    /// Tạo hoặc lấy Sprite bo tròn 9-slice từ bộ đệm
    /// </summary>
    public static Sprite GetRoundedBoxSprite(Color fill, Color border, float borderWidth = 2.5f, int radius = 18, int size = 64)
    {
        string key = $"round_{fill}_{border}_{borderWidth}_{radius}_{size}";
        if (spriteCache.TryGetValue(key, out Sprite cached) && cached != null)
        {
            return cached;
        }

        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        float cx = (size - 1) * 0.5f;
        float cy = (size - 1) * 0.5f;
        float halfBox = cx - radius;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float qx = Mathf.Max(0f, Mathf.Abs(x - cx) - halfBox);
                float qy = Mathf.Max(0f, Mathf.Abs(y - cy) - halfBox);
                float dist = Mathf.Sqrt(qx * qx + qy * qy) - radius;

                // Anti-aliased outer edge
                float alpha = Mathf.Clamp01(0.5f - dist);

                if (alpha <= 0.001f)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                // Inner vs Border determination
                Color pixelColor;
                if (dist > -borderWidth)
                {
                    // Border area
                    float borderFade = Mathf.Clamp01((-dist) / borderWidth);
                    pixelColor = Color.Lerp(border, fill, borderFade * 0.35f);
                }
                else
                {
                    // Fill area: subtle inner gradient (slightly darker towards edges)
                    float centerDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy)) / cx;
                    Color gradientFill = Color.Lerp(fill, fill * 0.82f, centerDist * 0.5f);
                    pixelColor = gradientFill;
                }

                pixelColor.a *= alpha;
                tex.SetPixel(x, y, pixelColor);
            }
        }

        tex.Apply();

        Vector4 borderSlice = new Vector4(radius + 2, radius + 2, radius + 2, radius + 2);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, borderSlice);
        spriteCache[key] = sprite;
        return sprite;
    }

    /// <summary>
    /// Trang trí nút bấm với viền bo tròn, hiệu ứng đổi màu hover/press kinh dị
    /// </summary>
    public static void ApplyHorrorButtonStyle(Button btn, Color baseFill, Color baseBorder)
    {
        if (btn == null) return;

        Image img = btn.GetComponent<Image>();
        if (img == null) img = btn.gameObject.AddComponent<Image>();

        img.sprite = GetRoundedBoxSprite(baseFill, baseBorder, 2.5f, 16);
        img.type = Image.Type.Sliced;
        img.color = Color.white;
        img.raycastTarget = true;

        btn.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.25f, 1.25f, 1.25f, 1f); // Sáng bừng lên khi rê chuột
        colors.pressedColor = new Color(0.75f, 0.55f, 0.55f, 1f);      // Đậm lại khi nhấn
        colors.selectedColor = Color.white;
        colors.fadeDuration = 0.12f;
        btn.colors = colors;
    }

    /// <summary>
    /// Thêm hiệu ứng viền bóng đen đổ kinh dị cho chữ Text
    /// </summary>
    public static void AddHorrorTextShadow(Text text, Color shadowColor, Vector2 distance)
    {
        if (text == null) return;

        Shadow shadow = text.GetComponent<Shadow>();
        if (shadow == null) shadow = text.gameObject.AddComponent<Shadow>();

        shadow.effectColor = shadowColor;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    /// <summary>
    /// Thêm hiệu ứng Outline ma mị cho tiêu đề
    /// </summary>
    public static void AddHorrorTextOutline(Text text, Color outlineColor, Vector2 distance)
    {
        if (text == null) return;

        Outline outline = text.GetComponent<Outline>();
        if (outline == null) outline = text.gameObject.AddComponent<Outline>();

        outline.effectColor = outlineColor;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }
}
