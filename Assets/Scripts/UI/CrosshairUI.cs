using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple crosshair UI displayed at screen center
/// Changes color when looking at interactable objects
/// </summary>
public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [Tooltip("Crosshair image (should be anchored to screen center)")]
    public Image crosshairImage;
    
    [Tooltip("Normal color (when not looking at interactable)")]
    public Color normalColor = new Color(0.78f, 0.74f, 0.65f, 0.75f);
    
    [Tooltip("Highlight color (when looking at interactable)")]
    public Color highlightColor = new Color(0.72f, 0.04f, 0.07f, 1f);
    
    [Tooltip("Size of crosshair (width and height)")]
    public float crosshairSize = 34f;

    private MirrorHouseHorrorHUD horrorHUD;
    
    void Start()
    {
        horrorHUD = FindAnyObjectByType<MirrorHouseHorrorHUD>();

        // The horror HUD owns the reticle in Level_Main. Disable the old Image
        // so its sprite-less fallback cannot render as a large coloured square.
        if (horrorHUD != null)
        {
            SetLegacyCrosshairVisible(false);
        }
        // Keep the original simple crosshair only as a fallback for scenes that
        // do not use MirrorHouseHorrorHUD.
        else if (crosshairImage == null)
        {
            CreateCrosshairImage();
        }
        
        // Set initial color
        SetNormal();
    }
    
    /// <summary>
    /// Set crosshair to normal state (not looking at interactable)
    /// </summary>
    public void SetNormal()
    {
        if (TryGetHorrorHUD(out MirrorHouseHorrorHUD hud))
        {
            SetLegacyCrosshairVisible(false);
            hud.SetCrosshairHighlighted(false);
            return;
        }

        if (crosshairImage != null)
        {
            crosshairImage.color = normalColor;
        }
    }
    
    /// <summary>
    /// Set crosshair to highlight state (looking at interactable)
    /// </summary>
    public void SetHighlight()
    {
        if (TryGetHorrorHUD(out MirrorHouseHorrorHUD hud))
        {
            SetLegacyCrosshairVisible(false);
            hud.SetCrosshairHighlighted(true);
            return;
        }

        if (crosshairImage != null)
        {
            crosshairImage.color = highlightColor;
        }
    }

    private bool TryGetHorrorHUD(out MirrorHouseHorrorHUD hud)
    {
        if (horrorHUD == null)
        {
            horrorHUD = FindAnyObjectByType<MirrorHouseHorrorHUD>();
        }

        hud = horrorHUD;
        return hud != null;
    }

    private void SetLegacyCrosshairVisible(bool visible)
    {
        if (crosshairImage != null)
        {
            crosshairImage.enabled = visible;
        }
    }
    
    /// <summary>
    /// Create a simple crosshair image if none exists
    /// </summary>
    private void CreateCrosshairImage()
    {
        // Create UI Image component on this GameObject
        crosshairImage = gameObject.AddComponent<Image>();
        
        // Set to simple white sprite
        crosshairImage.sprite = null;
        crosshairImage.color = normalColor;
        
        // Set size
        RectTransform rectTransform = crosshairImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(crosshairSize, crosshairSize);
        
        // Center on screen
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        Debug.Log("[CrosshairUI] Auto-created crosshair image");
    }
}
