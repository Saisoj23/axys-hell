using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    Camera cam;
    Animator anim;
    AudioSource sfx;
    GameController game;
    MusicAndData musicAndData;

    [Header("Sounds")]
    public List<AudioClip> sounds;

    [Header("Buttons")]
    public Button playButton;
    public Button pauseButton;
    public Toggle musicToggle;
    public CanvasGroup hudButtons;
    [Header("Images")]
    public Image bestImage;
    public Image timeImage;
    public Image pauseImage;
    [Header("Texts")]
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text bestText;
    public TMPro.TMP_Text subTittleText;

    void Start()
    {
        anim = GetComponent<Animator>();
        game = GetComponent<GameController>();
        cam = GetComponentInChildren<Camera>();
        sfx = GetComponentInChildren<AudioSource>();
        musicAndData = GameObject.FindObjectOfType<MusicAndData>();
        if (Screen.width > Screen.height)
        {
            cam.orthographicSize *= Mathf.InverseLerp(0f, Screen.width, Screen.height);
        }
        bestText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        sfx.enabled = false;
        musicToggle.isOn = PlayerPrefs.GetInt("Music", 1) == 1 ? false : true;
        sfx.enabled = true;
    }

    void Update()
    {
        timeText.text = ((int)game.score).ToString();
        bestText.text = ((int)game.bestScore).ToString();
    }

    public void Play (bool active)
    {
        pauseButton.enabled = active;
        playButton.enabled = !active;
        hudButtons.interactable = !active;
        anim.SetBool("Playing", active);
        this.enabled = active;
    }

    public void Pause (bool pause)
    {
        subTittleText.SetText("Tap here to resume");
        anim.SetBool("Pause", pause);
        playButton.enabled = pause;
    }

    public void Stop ()
    {
        SFXPlay(1);
        anim.SetBool("Playing", false);
        subTittleText.SetText("Tap here to restart");
        this.enabled = false;
    }

    public void SetMusic ()
    {
        if (musicAndData != null) musicAndData.SetMusic(musicToggle.isOn);
    }

    public void SFXPlay (int sound)
    {
        if (sound == 2 && game.pause)
        {
            sfx.clip = sounds[1];
        }
        else
        {
            sfx.clip = sounds[sound];
        }
        sfx.Play();
    }
}
