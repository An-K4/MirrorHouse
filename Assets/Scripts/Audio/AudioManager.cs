using UnityEngine;

/// <summary>
/// AUDIO MANAGER - Quản lý toàn bộ âm thanh và nhạc nền cho MirrorHouse
/// Hỗ trợ cả AudioClip gán từ Inspector và Procedural Audio tổng hợp tự động khi chưa có asset.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips (Tùy chọn - Tự động tạo âm thanh nếu để trống)")]
    [SerializeField] private AudioClip ambientBgm;
    [SerializeField] private AudioClip jumpscareSfx;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip keyPickupSfx;
    [SerializeField] private AudioClip lockOpenSfx;
    [SerializeField] private AudioClip cluePickupSfx;
    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private AudioClip buttonClickSfx;

    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("Footstep Settings")]
    [SerializeField] private float walkStepInterval = 0.55f;
    [SerializeField] private float sprintStepInterval = 0.35f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource footstepSource;

    private PlayerController player;
    private float footstepTimer;
    private bool isPlayerSprinting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volume preferences
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetupAudioSources();
        GenerateProceduralClipsIfNeeded();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerCaught += HandlePlayerCaught;
        GameEvents.OnGameWin += HandleGameWin;
        GameEvents.OnKeyPickup += HandleKeyPickup;
        GameEvents.OnLockOpened += HandleLockOpened;
        GameEvents.OnCodeCorrect += HandleCodeCorrect;
        GameEvents.OnCluePickup += HandleCluePickup;
        GameEvents.OnPlayerSprintChanged += HandleSprintChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerCaught -= HandlePlayerCaught;
        GameEvents.OnGameWin -= HandleGameWin;
        GameEvents.OnKeyPickup -= HandleKeyPickup;
        GameEvents.OnLockOpened -= HandleLockOpened;
        GameEvents.OnCodeCorrect -= HandleCodeCorrect;
        GameEvents.OnCluePickup -= HandleCluePickup;
        GameEvents.OnPlayerSprintChanged -= HandleSprintChanged;
    }

    private void Start()
    {
        PlayAmbientMusic();
    }

    private void Update()
    {
        UpdateFootsteps();
    }

    private void SetupAudioSources()
    {
        // BGM Source
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = musicVolume * masterVolume;

        // SFX Source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume * masterVolume;

        // Footstep Source
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = false;
        footstepSource.playOnAwake = false;
        footstepSource.volume = sfxVolume * masterVolume * 0.45f;
    }

    private void UpdateFootsteps()
    {
        if (Time.timeScale <= 0f) return;

        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
            if (player == null) return;
        }

        if (player.IsMoving())
        {
            float interval = isPlayerSprinting ? sprintStepInterval : walkStepInterval;
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= interval)
            {
                footstepTimer = 0f;
                PlayFootstep();
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    public void PlayFootstep()
    {
        if (footstepSfx == null) return;

        footstepSource.pitch = Random.Range(0.85f, 1.15f);
        footstepSource.PlayOneShot(footstepSfx, footstepSource.volume);
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeScale);
        }
    }

    public void PlayButtonClick()
    {
        PlayOneShot(buttonClickSfx, 0.7f);
    }

    public void PlayLockOpen()
    {
        PlayOneShot(lockOpenSfx, 0.9f);
    }

    public void PlayKeyPickup()
    {
        PlayOneShot(keyPickupSfx, 0.8f);
    }

    public void PlayCluePickup()
    {
        PlayOneShot(cluePickupSfx, 0.8f);
    }

    public void PlayAmbientMusic()
    {
        if (ambientBgm != null && bgmSource != null)
        {
            bgmSource.clip = ambientBgm;
            bgmSource.volume = musicVolume * masterVolume;
            bgmSource.Play();
        }
    }

    public void StopMusic()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    // ================= Volume Setters =================
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        UpdateSourceVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        UpdateSourceVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        UpdateSourceVolumes();
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    private void UpdateSourceVolumes()
    {
        if (bgmSource != null) bgmSource.volume = musicVolume * masterVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume * masterVolume;
        if (footstepSource != null) footstepSource.volume = sfxVolume * masterVolume * 0.45f;
    }

    // ================= Event Handlers =================
    private void HandlePlayerCaught()
    {
        StopMusic();
        PlayOneShot(jumpscareSfx, 1.2f);
    }

    private void HandleGameWin()
    {
        StopMusic();
        PlayOneShot(victorySfx, 1.0f);
    }

    private void HandleKeyPickup(string keyId)
    {
        PlayOneShot(keyPickupSfx, 0.8f);
    }

    private void HandleLockOpened(string lockId)
    {
        PlayOneShot(lockOpenSfx, 0.9f);
    }

    private void HandleCodeCorrect(string lockId)
    {
        PlayOneShot(lockOpenSfx, 0.9f);
    }

    private void HandleCluePickup(string clueId, string text)
    {
        PlayOneShot(cluePickupSfx, 0.8f);
    }

    private void HandleSprintChanged(bool sprinting)
    {
        isPlayerSprinting = sprinting;
    }

    // ================= Procedural Audio Generators =================
    // Tự động tổng hợp âm thanh tại runtime nếu chưa có audio clip nào được gán
    private void GenerateProceduralClipsIfNeeded()
    {
        if (footstepSfx == null)
            footstepSfx = GenerateFootstepClip();

        if (buttonClickSfx == null)
            buttonClickSfx = GenerateClickClip();

        if (keyPickupSfx == null)
            keyPickupSfx = GenerateChimeClip(520f, 780f, 0.4f);

        if (lockOpenSfx == null)
            lockOpenSfx = GenerateClickClip();

        if (cluePickupSfx == null)
            cluePickupSfx = GenerateRustleClip();

        if (jumpscareSfx == null)
            jumpscareSfx = GenerateJumpscareClip();

        if (victorySfx == null)
            victorySfx = GenerateVictoryClip();

        if (ambientBgm == null)
            ambientBgm = GenerateAmbientDroneClip();
    }

    private AudioClip GenerateFootstepClip()
    {
        int sampleRate = 22050;
        int length = (int)(sampleRate * 0.15f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / length;
            float env = Mathf.Exp(-t * 22f);
            float freq = Mathf.Sin(2f * Mathf.PI * 65f * (i / (float)sampleRate));
            float noise = (Random.value * 2f - 1f) * 0.4f;
            samples[i] = (freq + noise) * env * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Footstep", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateClickClip()
    {
        int sampleRate = 22050;
        int length = (int)(sampleRate * 0.05f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / length;
            float env = 1f - t;
            float freq = Mathf.Sin(2f * Mathf.PI * 900f * (i / (float)sampleRate));
            samples[i] = freq * env * 0.4f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Click", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateChimeClip(float f1, float f2, float dur)
    {
        int sampleRate = 22050;
        int length = (int)(sampleRate * dur);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / length;
            float env = Mathf.Exp(-t * 6f);
            float s1 = Mathf.Sin(2f * Mathf.PI * f1 * (i / (float)sampleRate));
            float s2 = Mathf.Sin(2f * Mathf.PI * f2 * (i / (float)sampleRate));
            samples[i] = (s1 * 0.6f + s2 * 0.4f) * env * 0.45f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Chime", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateRustleClip()
    {
        int sampleRate = 22050;
        int length = (int)(sampleRate * 0.25f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / length;
            float env = Mathf.Sin(t * Mathf.PI);
            float noise = (Random.value * 2f - 1f) * 0.5f;
            samples[i] = noise * env * 0.35f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Rustle", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateJumpscareClip()
    {
        int sampleRate = 22050;
        int length = (int)(sampleRate * 1.5f);
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / length;
            float env = t < 0.05f ? t / 0.05f : Mathf.Exp(-t * 2f);
            float fLow = Mathf.Sin(2f * Mathf.PI * 55f * (i / (float)sampleRate));
            float fDissonant = Mathf.Sin(2f * Mathf.PI * (230f + Random.Range(-20f, 20f)) * (i / (float)sampleRate));
            float fHigh = Mathf.Sin(2f * Mathf.PI * 1400f * (i / (float)sampleRate));
            float noise = (Random.value * 2f - 1f) * 0.6f;

            samples[i] = Mathf.Clamp((fLow * 0.4f + fDissonant * 0.4f + fHigh * 0.2f + noise) * env * 0.85f, -1f, 1f);
        }

        AudioClip clip = AudioClip.Create("Procedural_Jumpscare", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateVictoryClip()
    {
        int sampleRate = 22050;
        int length = (int)(sampleRate * 2.5f);
        float[] samples = new float[length];

        float[] notes = { 440f, 554.37f, 659.25f, 880f }; // A major arpeggio
        int noteLen = length / notes.Length;

        for (int i = 0; i < length; i++)
        {
            int noteIdx = Mathf.Min(i / noteLen, notes.Length - 1);
            float noteFreq = notes[noteIdx];
            float noteT = (float)(i % noteLen) / noteLen;
            float env = Mathf.Exp(-noteT * 3.5f);
            float sine = Mathf.Sin(2f * Mathf.PI * noteFreq * (i / (float)sampleRate));
            samples[i] = sine * env * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Victory", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateAmbientDroneClip()
    {
        int sampleRate = 22050;
        int length = sampleRate * 8; // 8 seconds loop
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float timeSec = (float)i / sampleRate;
            float drone1 = Mathf.Sin(2f * Mathf.PI * 45f * timeSec);
            float drone2 = Mathf.Sin(2f * Mathf.PI * 67.5f * timeSec + Mathf.Sin(timeSec * 0.5f));
            float sub = Mathf.Sin(2f * Mathf.PI * 30f * timeSec) * 0.5f;
            float noise = (Random.value * 2f - 1f) * 0.08f;
            samples[i] = (drone1 * 0.35f + drone2 * 0.25f + sub + noise) * 0.4f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Ambient_Drone", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
