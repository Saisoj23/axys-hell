using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicAndData : MonoBehaviour
{
    float camSize;
    float ratio;
    Camera cam;
    AudioSource music;

    public AudioMixer mixer;

    public Animator intro;

    public float fadeTime;

    public AudioClip[] clips;
    public SpawnController[] spawns;

    public bool EnterInCustomLevel = false;
    
    void Awake()
    {
        music = GetComponent<AudioSource>();
        if (FindObjectsOfType<MusicAndData>().Length > 1)
        {
            intro.SetTrigger("Skip");
            Destroy(gameObject);
        }
        else 
        {
            DontDestroyOnLoad(gameObject);  
        }
    }

    void Update()
    {
        #if !UNITY_ANDROID || UNITY_EDITOR
        if (Screen.width > Screen.height)
        {
            ScreenUpdate();
        }
        else
        {
            cam.orthographicSize = camSize;
            Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode);
        }
        #endif
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

    public void PlayMusic(bool active)
    {
        if (active && !music.isPlaying) 
        {
            music.volume = 1f;
            music.Play();
        }
        else if (!active) StartCoroutine(MusicFade());
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

    IEnumerator MusicFade ()
    {
        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            music.volume = Mathf.InverseLerp(fadeTime, 0f, i);
            yield return null;
        }
        music.Stop();
    }

    public void UpdateCam ()
    {
        cam = Camera.main;
        camSize = cam.orthographicSize;
    }

    void ScreenUpdate ()
    {
        print(cam.orthographicSize);
        print(camSize);
        ratio = (float)Screen.height / (float)Screen.width;
        cam.orthographicSize = camSize * ratio;
        Screen.SetResolution((int)(Screen.width * ratio), (int)(Screen.height * ratio), Screen.fullScreenMode);
    }
}
