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
        if (mode == LoadSceneMode.Additive)
            return;

        string sceneName = scene.name.ToLower();

        // Cutscene scenes: no music change
        if (sceneName.Contains("cutscene"))
            return;

        if (sceneName.Contains("main"))
        {
            if (musicSource.clip != mainMenuMusic)
                PlayMusic(mainMenuMusic);
        }
        else if (sceneName.Contains("level3"))
        {
            if (musicSource.clip != level3Music)
                PlayMusic(level3Music);
        }
        else if (sceneName.Contains("level2"))
        {
            if (musicSource.clip != level2Music)
                PlayMusic(level2Music);
        }
        else if (sceneName.Contains("level"))
        {
            if (musicSource.clip != levelMusic)
                PlayMusic(levelMusic);
        }
        else
        {
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