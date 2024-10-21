using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour {
    
    public AudioMixer mixer;

    [SerializeField] private Slider music;
    [SerializeField] private Slider effects;
    [SerializeField] private Slider zoomSpeed;

    public void UpdateSlider() {
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        effects.value = PlayerPrefs.GetFloat("EffectVolume", 1);
        zoomSpeed.value = PlayerPrefs.GetFloat("ZoomSpeedMouse", 0);
    }

    public void SetLevelM (float sliderValue)
    {
        mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetLevelE (float sliderValue)
    {
        mixer.SetFloat("Effect", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("EffectVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSens (float sliderValue) {
        PlayerPrefs.SetFloat("ZoomSpeedMouse", sliderValue * 2);
        PlayerPrefs.Save();
    }
}