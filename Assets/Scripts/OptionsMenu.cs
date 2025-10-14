using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private GameObject optionsPanel;
    
    void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("Music", 0.5f);
        }
        if (brightnessSlider != null)
        {
            brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        }
    }
    
    public void SetMusic(float volume)
    {
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