using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ConfigMenu : MonoBehaviour
{
    public Image difficultyImage;
    public TMPro.TMP_Text difficultyText;
    bool difficulty;
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Button sliceButton;
    public Button pointButton;
    public Button sliceSprite;
    public Button pointSprite;
    public Button sensibilitySprite;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider sensibilitySlider;

    public Sprite[] difficultySprites;

    public AudioMixer mixer;

    public AudioSource aud; 

    void Awake()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty", 0) == 1 ? true : false;
        if (difficulty)
        {
            difficultyImage.sprite = difficultySprites[1];
            difficultyText.text = "Hard";
        }
        else
        {
            difficultyImage.sprite = difficultySprites[0];
            difficultyText.text = "Normal";
        }

        aud.enabled = false;
        musicToggle.isOn = PlayerPrefs.GetInt("Music", 1) == 1 ? false : true;
        musicSlider.value = PlayerPrefs.GetFloat("MusicValue", 1);
        musicSlider.interactable = !musicToggle.isOn;

        soundToggle.isOn = PlayerPrefs.GetInt("Sound", 1) == 1 ? false : true;
        soundSlider.value = PlayerPrefs.GetFloat("SoundValue", 1);
        soundSlider.interactable = !soundToggle.isOn;

        if (PlayerPrefs.GetInt("ControlMode", 1) == 1)
        {
            sliceSprite.interactable = true;
            pointSprite.interactable = false;
        }
        else 
        {
            sliceSprite.interactable = false;
            pointSprite.interactable = true;
        }

        sensibilitySlider.value = PlayerPrefs.GetFloat("SensibilityValue", 1);
        if (sliceSprite.interactable)
        {
            sensibilitySprite.interactable = true;;
            sensibilitySlider.interactable = true;
        }
        else
        {
            sensibilitySprite.interactable = false;
            sensibilitySlider.interactable = false;
        }
        aud.enabled = true;
    }

    public void SetDifficulty ()
    {
        if (difficulty)
        {
            PlayerPrefs.SetInt("Difficulty", 0);
            difficultyImage.sprite = difficultySprites[0];
            difficultyText.text = "Normal";
        }
        else
        {
            PlayerPrefs.SetInt("Difficulty", 1);
            difficultyImage.sprite = difficultySprites[1];
            difficultyText.text = "Hard";
        }
        difficulty = !difficulty;
    }

    public void SetMusic ()
    {
        PlayerPrefs.SetInt("Music", musicToggle.isOn ? 0 : 1);
        mixer.SetFloat("MusicVolume", musicToggle.isOn ? -80f : Mathf.Log10(PlayerPrefs.GetFloat("MusicValue", 1f)) * 20);

        musicSlider.interactable = !musicToggle.isOn;
    }

    public void SetSound ()
    {
        PlayerPrefs.SetInt("Sound", soundToggle.isOn ? 0 : 1);
        mixer.SetFloat("SoundVolume", soundToggle.isOn ? -80f : Mathf.Log10(PlayerPrefs.GetFloat("SoundValue", 1f)) * 20);

        soundSlider.interactable = !soundToggle.isOn;
    }

    public void SetMusicValue ()
    {
        PlayerPrefs.SetFloat("MusicValue", musicSlider.value);
        if (!musicToggle.isOn) mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicValue", 1f)) * 20);
    }

    public void SetSoundValue ()
    {
        PlayerPrefs.SetFloat("SoundValue", soundSlider.value);
        if (!soundToggle.isOn) mixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundValue", 1f)) * 20);
    }

    public void SetSliceOrPoint (bool active)
    {
        PlayerPrefs.SetInt("ControlMode", active ? 1 : 0);
        if (active)
        {
            sliceSprite.interactable = true;
            pointSprite.interactable = false;
            sensibilitySlider.interactable = true;
            sensibilitySprite.interactable = true;
        }
        else 
        {
            sliceSprite.interactable = false;
            pointSprite.interactable = true;
            sensibilitySlider.interactable = false;
            sensibilitySprite.interactable = false;
        }
    }

    public void SetSensibilityValue ()
    {
        PlayerPrefs.SetFloat("SensibilityValue", sensibilitySlider.value);
    }
}
