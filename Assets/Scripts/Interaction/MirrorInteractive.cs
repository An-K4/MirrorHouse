using UnityEngine;

/// <summary>
/// MIRROR INTERACTIVE - Tương tác với các tấm gương kinh dị trong "Nhà Gương"
/// 
/// Khi người chơi lại gần hoặc bấm E / Chạm vào gương:
/// - Hiện suy nghĩ tâm lý rùng rợn đặc trưng của Mirror House.
/// - Phát âm thanh ghê rợn (thì thầm, nhịp tim, kính nứt nhẹ).
/// - Có tỉ lệ làm bóng ma trong gương chớp nháy hoặc rùng mình.
/// </summary>
public class MirrorInteractive : Interactable
{
    [Header("Mirror Horror Quotes")]
    [Tooltip("Danh sách câu thoại tâm lý khi nhìn vào tấm gương")]
    [SerializeField] [TextArea(2, 4)]
    private string[] horrorQuotes = new string[]
    {
        "Chiếc bóng trong gương dường như chuyển động chậm hơn bạn một nhịp...",
        "Bạn nhìn thẳng vào mắt mình trong gương, nhưng có cảm giác người bên trong đang quan sát bạn từ một thế giới khác...",
        "Một làn hơi thở lạnh buốt phả mờ mặt kính... từ phía BÊN TRONG tấm gương.",
        "Dưới lớp tráng thủy bạc, bạn thấy những vết cào móng tay chằng chịt cố cào ra ngoài.",
        "\"Đừng quay lưng lại với ta...\" - Một tiếng thì thầm vô hình như vọng ra từ sâu thẳm trong gương.",
        "Hình ảnh phản chiếu của bạn mỉm cười, dù khuôn mặt bạn đang tái mét vì sợ hãi.",
        "Tấm gương rung rinh nhẹ như mặt nước đen thẫm, như thể có bàn tay đang áp vào từ phía bên kia..."
    };

    [Header("Custom Mirror Quote (Optional)")]
    [SerializeField] [TextArea(2, 4)]
    private string customQuote = "";

    [Header("Audio Cue")]
    [SerializeField] private AudioClip inspectSound;
    private AudioSource audioSource;
    private int inspectCount = 0;

    protected override void Start()
    {
        base.Start();
        interactionRange = 2.2f;
        if (string.IsNullOrEmpty(interactionPrompt) || interactionPrompt == "Press E to interact")
        {
            interactionPrompt = "Nhìn vào gương [E / Chạm]";
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            audioSource.maxDistance = 6f;
        }
    }

    public void SetCustomQuote(string quote)
    {
        customQuote = quote;
    }

    protected override void Interact()
    {
        inspectCount++;

        string selectedText;
        if (!string.IsNullOrEmpty(customQuote))
        {
            selectedText = customQuote;
        }
        else if (horrorQuotes != null && horrorQuotes.Length > 0)
        {
            int index = Random.Range(0, horrorQuotes.Length);
            selectedText = horrorQuotes[index];
        }
        else
        {
            selectedText = "Chiếc bóng trong gương chằm chằm nhìn bạn...";
        }

        // Play SFX
        if (inspectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(inspectSound);
        }
        else if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // Hiển thị text kinh dị lên màn hình qua GameEvents
        string clueId = "mirror_" + gameObject.name.ToLower();
        GameEvents.TriggerCluePickup(clueId, $"[TẤM GƯƠNG KỲ QUÁI]\n\n{selectedText}");

        Debug.Log($"<color=magenta>[MIRROR] Người chơi nhìn vào '{gameObject.name}': {selectedText}</color>");
    }
}
