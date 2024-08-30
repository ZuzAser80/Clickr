using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour {
    
    public AudioMixer mixer;

    [SerializeField] private Slider music;
    [SerializeField] private Slider effects;

    public void UpdateSlider() {
        //Debug.Log(" : " + PlayerPrefs.GetFloat("MusicVolume") + " : " + PlayerPrefs.GetFloat("EffectVolume"));
        music.value = PlayerPrefs.GetFloat("MusicVolume");
        effects.value = PlayerPrefs.GetFloat("EffectVolume");
    }

    public void SetLevelM (float sliderValue)
    {
        //Debug.Log("SetLevelM");
        mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetLevelE (float sliderValue)
    {
        //Debug.Log("SetLevelE");
        mixer.SetFloat("Effect", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("EffectVolume", sliderValue);
        PlayerPrefs.Save();
    }
}