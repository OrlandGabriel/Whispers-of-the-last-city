using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public static MusicManager Instance => instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip levelMusic; // Used for all game levels

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;

            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

            SetMusicVolume(musicVol);
            SetSFXVolume(sfxVol);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ignore additive loads
        if (mode == LoadSceneMode.Additive)
            return;

        string sceneName = scene.name.ToLower();

        // Cutscene scenes: no music change
        if (sceneName.Contains("cutscene"))
            return;

        // Only switch music if we’re in main menu or game
        if (sceneName.Contains("main"))
        {
            // If we're already playing this track, don't restart
            if (musicSource.clip != mainMenuMusic)
                PlayMusic(mainMenuMusic);
        }
        else if (sceneName.Contains("level"))
        {
            // If we're already playing the same level track, don't restart
            if (musicSource.clip != levelMusic)
                PlayMusic(levelMusic);
        }
        else
        {
            // Default behavior — continue playing current track
            if (!musicSource.isPlaying)
                musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        // Already playing this clip? do nothing
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, sfxSource.volume);
    }
}
