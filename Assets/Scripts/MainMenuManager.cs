using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject optionsPanel;

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public Slider musicSlider;

    private void Start()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // Nilai slider terakhir dari PlayerPrefs (default: 0.75)
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.75f);

        if (masterSlider != null) masterSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;

        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat("MasterVol", dB);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat("MusicVol", dB);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari game");
        Application.Quit();
    }
}