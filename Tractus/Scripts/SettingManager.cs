using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    public const string BGM_VOLUME_KEY = "BGMVolume";
    public const string SFX_VOLUME_KEY = "SFXVolume";

    void Start()
    {
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        LoadSettings();
    }

    public void SetBGMVolume(float volume)
    {
        float db = Mathf.Log10(volume) * 20;
        mainMixer.SetFloat(BGM_VOLUME_KEY, db);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        float db = Mathf.Log10(volume) * 20;
        mainMixer.SetFloat(SFX_VOLUME_KEY, db);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
    }

    private void LoadSettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);

        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
        
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }
}