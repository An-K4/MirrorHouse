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
    public Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    
    [Tooltip("Highlight color (when looking at interactable)")]
    public Color highlightColor = new Color(1f, 0.8f, 0f, 1f);
    
    [Tooltip("Size of crosshair (width and height)")]
    public float crosshairSize = 20f;
    
    void Start()
    {
        // If no image assigned, create one
        if (crosshairImage == null)
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
        if (crosshairImage != null)
        {
            crosshairImage.color = highlightColor;
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
