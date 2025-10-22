using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.3f;

    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f; // 2D sound
        musicSource.volume = musicVolume;

        // Start playing if clip assigned
        if (backgroundMusic != null)
            PlayMusic(backgroundMusic);
    }

    // --- Music ---
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    // --- Sound Effects ---
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        StartCoroutine(PlaySFXCoroutine(clip, volume));
    }

    private IEnumerator PlaySFXCoroutine(AudioClip clip, float volume)
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.Play();

        yield return new WaitForSeconds(clip.length);
        Destroy(src);
    }
}
