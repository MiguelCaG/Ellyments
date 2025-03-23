using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider soundFXSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        float soundFXVolume = PlayerPrefs.GetFloat("soundFXVolume", 0.5f);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);

        //Debug.Log($"{masterVolume}, {soundFXVolume}, {musicVolume}");

        SetMasterVolume(masterVolume);
        SetSoundFXVolume(soundFXVolume);
        SetMusicVolume(musicVolume);

        masterSlider.value = masterVolume;
        soundFXSlider.value = soundFXVolume;
        musicSlider.value = musicVolume;
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("masterVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("soundFXVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("musicVolume", level);
    }
}
