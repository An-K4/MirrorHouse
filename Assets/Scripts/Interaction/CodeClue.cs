using UnityEngine;

/// <summary>
/// CLUE SYSTEM - Gợi ý mã số cho Code Lock
/// 
/// ROLE C (Level Designer):
/// 1. Đặt clueId UNIQUE (VD: "clue_safe_01", "clue_door_hint")
/// 2. Viết clueText - gợi ý về mã số (VD: "The code is 12__", "Look at the painting")
/// 3. [TextArea] cho phép viết nhiều dòng trong Inspector
/// 
/// ROLE D (UI/SFX):
/// - Subscribe GameEvents.OnCluePickup(clueId, clueText) để:
///   + Hiện clue UI panel với text
///   + Add vào clue journal/notebook
///   + Play discovery sound
/// </summary>
public class CodeClue : Interactable
{
    [Header("Clue Settings")]
    [Tooltip("ROLE C: ID UNIQUE cho clue (VD: clue_safe_01, clue_door_hint)")]
    [SerializeField] private string clueId = "clue_safe_01";
    
    [Tooltip("ROLE C: Nội dung gợi ý - viết hint về mã số. TextArea cho phép nhiều dòng")]
    [SerializeField] [TextArea] private string clueText = "The code is 12__";
    
    protected override void Interact()
    {
        ShowClue();
    }
    
    void ShowClue()
    {
        // Trigger event cho Role D
        GameEvents.TriggerCluePickup(clueId, clueText);
        
        Debug.Log($"Clue: {clueText}");
        
        // Không destroy - player có thể đọc lại
    }
    
    public string GetClueText() => clueText;
}
