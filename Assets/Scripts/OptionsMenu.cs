using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private GameObject optionsPanel;
    
    void Start()
    {
        // Load saved settings and apply them
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("Music", 0.5f);
            SetMusic(musicSlider.value); // Apply the volume immediately
        }
        if (brightnessSlider != null)
        {
            brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
            SetBrightness(brightnessSlider.value); // Apply brightness immediately
        }
    }
    
public void SetMusic(float volume)
{
    Debug.Log("SetMusic called! Volume: " + volume); // ADD THIS LINE
    AudioListener.volume = volume;
    PlayerPrefs.SetFloat("Music", volume);
}
    
    public void SetBrightness(float brightness)
    {
        RenderSettings.ambientLight = Color.white * brightness;
        PlayerPrefs.SetFloat("Brightness", brightness);
    }
    
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}