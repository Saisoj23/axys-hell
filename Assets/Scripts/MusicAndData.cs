using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicAndData : MonoBehaviour
{
    AudioSource music;

    public AudioMixer mixer;

    public Animator intro;
    
    void Awake()
    {
        music = GetComponent<AudioSource>();
        if (GameObject.FindObjectsOfType<MusicAndData>().Length > 1)
        {
            intro.SetTrigger("Skip");
            Destroy(gameObject);
        }
        else 
        {
            DontDestroyOnLoad(gameObject);  
        }
    }

    public void Start ()
    {
        if (PlayerPrefs.GetInt("Music", 1) == 1 ? true : false)
        {
            print ("music " + Mathf.Log10(PlayerPrefs.GetFloat("MusicValue", 1f)) * 20);
            mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicValue", 1f)) * 20);
        }
        else
        {
            mixer.SetFloat("MusicVolume", -80f);
        }

        if (PlayerPrefs.GetInt("Sound", 1) == 1 ? true : false)
        {
            print ("sound " + Mathf.Log10(PlayerPrefs.GetFloat("SoundValue", 1f)) * 20);
            mixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundValue", 1f)) * 20);
        }
        else
        {
            mixer.SetFloat("SoundVolume", -80f);
        }
    }

    public void SetMusic(bool active)
    {
        if (!active)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicValue", 1f)) * 20);
            PlayerPrefs.SetInt("Music", 1);
        }
        else
        {
            mixer.SetFloat("MusicVolume", -80f);
            PlayerPrefs.SetInt("Music", 0);
        }
    }
}
