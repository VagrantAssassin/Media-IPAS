using UnityEngine;

public class AudioController : MonoBehaviour
{
    public enum BgmMode { Menu, Materi, Simulasi }

    public static AudioController Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip menuBgm;
    [SerializeField] private AudioClip materiBgm;
    [SerializeField] private AudioClip simulasiBgm;
    [SerializeField] private AudioClip buttonClickSfx;

    [Header("Volumes (0..1)")]
    [Range(0f, 1f)][SerializeField] private float menuBgmVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float materiBgmVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float simulasiBgmVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float clickSfxVolume = 1f;

    [Header("State")]
    [SerializeField] private bool muted = false;

    private BgmMode currentMode = BgmMode.Menu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        ApplyVolumes();
    }

    public void SetMode(BgmMode mode)
    {
        currentMode = mode;

        var clip = GetClipForMode(mode);
        if (bgmSource == null) return;

        // Jika clip sama dan sedang play, jangan restart (biar tidak terpotong)
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            ApplyVolumes();
            return;
        }

        bgmSource.clip = clip;

        if (clip != null)
            bgmSource.Play();

        ApplyVolumes();
    }

    public void PlayClick()
    {
        if (sfxSource == null || buttonClickSfx == null) return;

        // volume SFX diatur lewat sfxSource.volume (ApplyVolumes)
        sfxSource.PlayOneShot(buttonClickSfx, 1f);
    }

    public void ToggleMute()
    {
        muted = !muted;
        ApplyVolumes();
    }

    public bool IsMuted() => muted;

    public float GetMenuBgmVolume() => menuBgmVolume;
    public float GetMateriBgmVolume() => materiBgmVolume;
    public float GetSimulasiBgmVolume() => simulasiBgmVolume;
    public float GetClickSfxVolume() => clickSfxVolume;

    public void SetMenuBgmVolume(float v) { menuBgmVolume = Mathf.Clamp01(v); ApplyVolumes(); }
    public void SetMateriBgmVolume(float v) { materiBgmVolume = Mathf.Clamp01(v); ApplyVolumes(); }
    public void SetSimulasiBgmVolume(float v) { simulasiBgmVolume = Mathf.Clamp01(v); ApplyVolumes(); }
    public void SetClickSfxVolume(float v) { clickSfxVolume = Mathf.Clamp01(v); ApplyVolumes(); }

    private AudioClip GetClipForMode(BgmMode mode)
    {
        return mode switch
        {
            BgmMode.Menu => menuBgm,
            BgmMode.Materi => materiBgm,
            BgmMode.Simulasi => simulasiBgm,
            _ => null
        };
    }

    private float GetCurrentBgmChannelVolume()
    {
        return currentMode switch
        {
            BgmMode.Menu => menuBgmVolume,
            BgmMode.Materi => materiBgmVolume,
            BgmMode.Simulasi => simulasiBgmVolume,
            _ => 1f
        };
    }

    private void ApplyVolumes()
    {
        float muteMul = muted ? 0f : 1f;

        if (bgmSource != null)
            bgmSource.volume = GetCurrentBgmChannelVolume() * muteMul;

        if (sfxSource != null)
            sfxSource.volume = clickSfxVolume * muteMul;
    }
}