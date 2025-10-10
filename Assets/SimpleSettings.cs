using UnityEngine;
using UnityEngine.UI;

public class SimpleSettings : MonoBehaviour
{
    public AudioSource musicSource;  // Assign your music AudioSource here
    public AudioSource[] soundSources; // Assign all other sound AudioSources here

    public Slider musicVolumeSlider;
    public Toggle muteToggle;

    private float previousMusicVolume;

    void Start()
    {
        // Initialize UI elements with current values
        musicVolumeSlider.value = musicSource.volume;
        muteToggle.isOn = false;
        previousMusicVolume = musicSource.volume;

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggled);
    }

    void OnMusicVolumeChanged(float volume)
    {
        if (!muteToggle.isOn)
        {
            musicSource.volume = volume;
        }
        previousMusicVolume = volume;
    }

    void OnMuteToggled(bool isMuted)
    {
        if (isMuted)
        {
            // Mute music and sounds
            musicSource.volume = 0f;
            foreach (var source in soundSources)
            {
                source.volume = 0f;
            }
        }
        else
        {
            // Restore volumes
            musicSource.volume = previousMusicVolume;
            foreach (var source in soundSources)
            {
                source.volume = 1f; // or store and restore individual volumes if needed
            }
        }
    }
}
