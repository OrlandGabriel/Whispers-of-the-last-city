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
    [SerializeField] private AudioClip levelMusic; // default level music
    [SerializeField] private AudioClip level2Music;
    [SerializeField] private AudioClip level3Music;

void Awake()
{
    Debug.Log("MusicManager Awake called in scene: " + SceneManager.GetActiveScene().name);
    
    if (instance == null)
    {
        Debug.Log("First instance - keeping this MusicManager");
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
        Debug.Log("Duplicate found - destroying this MusicManager");
        Destroy(gameObject);
        return;
    }
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (mode == LoadSceneMode.Additive)
        return;

    string sceneName = scene.name.ToLower().Trim();
    Debug.Log("Scene Loaded: " + sceneName);

    // Stop any previous music
    StopMusic();

    // Ignore cutscenes
    if (sceneName.Contains("cutscene"))
        return;

    if (sceneName.Contains("main"))
    {
        Debug.Log("Playing Main Menu Music");
        PlayMusic(mainMenuMusic);
    }
    else if (sceneName.Contains("level 3"))
    {
        Debug.Log("Playing Level 3 Music");
        PlayMusic(level3Music);
    }
    else if (sceneName.Contains("level 2"))
    {
        Debug.Log("Playing Level 2 Music");
        PlayMusic(level2Music);
    }
    else if (sceneName.Contains("level 1") || sceneName.Contains("level"))
    {
        Debug.Log("Playing Default Level Music");
        PlayMusic(levelMusic);
    }
    else
    {
        Debug.Log("No specific match — resuming last music if available");
        if (!musicSource.isPlaying)
            musicSource.Play();
    }
}

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

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