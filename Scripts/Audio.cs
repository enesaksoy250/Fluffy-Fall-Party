using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{

    public Slider slider;

    private void Start()
    {
        LoadAudio();
    }

    public void SetAudio(float value)
    {
        AudioListener.volume = value;
        SaveAudio();
    }

    public void SaveAudio()
    {
        PlayerPrefs.SetFloat("AudioVolume", AudioListener.volume);
    }
    public void LoadAudio()
    {
        if (PlayerPrefs.HasKey("AudioVolume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume");
            slider.value = PlayerPrefs.GetFloat("AudioVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("AudioVolume", 0.5f);
            AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume");
            slider.value = PlayerPrefs.GetFloat("AudioVolume");
        }
    }
}
