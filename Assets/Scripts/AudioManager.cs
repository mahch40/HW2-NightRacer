using System.Collections;
using UnityEngine;

// Music, engine sound (follows the game speed) and sound effects.
// Other scripts call AudioManager.Instance.PlayCoin() and so on.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources (2D, Play On Awake off)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource engineSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float duckedMusicVolume = 0.15f;
    [SerializeField] private float duckSpeed = 4f;

    [Header("Engine (follows GameSpeed)")]
    [SerializeField] private AudioClip engineClip;
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 55f;
    [SerializeField] private Vector2 enginePitch = new Vector2(0.8f, 1.8f);
    [SerializeField] private Vector2 engineVolume = new Vector2(0.25f, 0.6f);
    [SerializeField] private float engineSmoothing = 4f;

    [Header("Sound effects")]
    [SerializeField] private AudioClip[] crashClips;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip fuelClip;
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float coinVolume = 0.35f;
    [SerializeField, Range(0f, 1f)] private float fuelVolume = 1f;

    [Header("References (leave empty in the menu scene)")]
    [SerializeField] private PlayerHealth playerHealth;

    private float targetMusicVolume;
    private bool engineActive;
    private float engineLevel;

    private void Awake()
    {
        Instance = this;

        musicSource.loop = true;
        engineSource.loop = true;
        engineSource.clip = engineClip;
        targetMusicVolume = musicVolume;
    }

    private IEnumerator Start()
    {
        if (playerHealth != null) playerHealth.OnHit += PlayCrash;

        yield return null;   // let GameManager.Start() run first

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleState;
            HandleState(GameManager.Instance.State);
        }
        else
        {
            HandleState(GameState.Menu);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (playerHealth != null) playerHealth.OnHit -= PlayCrash;
        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= HandleState;
    }

    private void HandleState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                PlayMusic(menuMusic);
                targetMusicVolume = musicVolume;
                SetEngine(false);
                break;

            case GameState.Playing:
                PlayMusic(gameplayMusic);
                targetMusicVolume = musicVolume;
                SetEngine(true);
                break;

            case GameState.Paused:
                targetMusicVolume = duckedMusicVolume;
                engineActive = false;
                engineSource.Pause();
                break;

            case GameState.GameOver:
                targetMusicVolume = duckedMusicVolume;
                SetEngine(false);
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = 0f;   
        musicSource.Play();
    }

    private void SetEngine(bool on)
    {
        engineActive = on;
        if (on)
        {
            engineSource.UnPause();
            if (!engineSource.isPlaying) engineSource.Play();
        }
        else
        {
            engineSource.Stop();
        }
    }

    private void Update()
    {
        // Unscaled time, so the music still fades while the game is paused.
        musicSource.volume = Mathf.Lerp(musicSource.volume, targetMusicVolume, duckSpeed * Time.unscaledDeltaTime);

        if (engineActive)
        {
            float target = Mathf.InverseLerp(minSpeed, maxSpeed, GameSpeed.Current);
            engineLevel = Mathf.Lerp(engineLevel, target, engineSmoothing * Time.deltaTime);
            engineSource.pitch = Mathf.Lerp(enginePitch.x, enginePitch.y, engineLevel);
            engineSource.volume = Mathf.Lerp(engineVolume.x, engineVolume.y, engineLevel);
        }
    }

    public void PlayCrash()
    {
        foreach (AudioClip clip in crashClips)
            PlaySfx(clip);
    }
    public void PlayCoin() { PlaySfx(coinClip, coinVolume); }
    public void PlayFuel() { PlaySfx(fuelClip, fuelVolume); }
    public void PlayUiClick() { PlaySfx(uiClickClip); }

    private void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.pitch = Random.Range(0.95f, 1.05f);   
        sfxSource.PlayOneShot(clip, sfxVolume * volume);
    }
}