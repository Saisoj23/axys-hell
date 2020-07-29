using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class UIController : MonoBehaviour
{
    float nextAd;

    Camera cam;
    Animator anim;
    GameController game;
    MusicAndData musicAndData;
    AdController ad;

    [Header("Sounds")]
    public List<AudioClip> sounds;
    public AudioSource sfx;

    [Header("Buttons")]
    public Button playButton;
    public Button pauseButton;
    public Toggle musicToggle;
    public CanvasGroup hudButtons;
    public Button[] adButton;
    [Header("Images")]
    public Image bestImage;
    public Image timeImage;
    public Image pauseImage;
    [Header("Texts")]
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text bestText;
    public TMPro.TMP_Text subTittleText;
    public Color[] textColor;

    void Start()
    {
        anim = GetComponent<Animator>();
        game = GetComponent<GameController>();
        ad = GetComponent<AdController>();
        cam = GetComponentInChildren<Camera>();
        musicAndData = GameObject.FindObjectOfType<MusicAndData>();

        if (musicAndData != null) musicAndData.PlayMusic(false);

        if (Screen.width > Screen.height)
        {
            cam.orthographicSize *= Mathf.InverseLerp(0f, Screen.width, Screen.height);
        }
        bestText.text = ((int)game.score).ToString();
        timeText.text = ((int)game.bestScore).ToString();
        
        sfx.enabled = false;
        musicToggle.isOn = PlayerPrefs.GetInt("Music", 1) == 1 ? false : true;
        sfx.enabled = true;

        //adButton[0].interactable = false;
        //adButton[1].interactable = false;

        nextAd = Time.time + 60;
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

        if (Time.time > nextAd && game.custom) 
        {
            nextAd = Time.time + 60;
            ad.PlayAd(0);
        }
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

    public void AppartButton()
    {
        if (anim != null) anim.SetTrigger("ExitText");
    }

    public void SetColor (int text, int color)
    {
        if (text == 0)
        {
            if (color == 0) bestText.color = Color.white;
            else bestText.color = textColor[text];
        }
        else
        {
            if (color == 0) timeText.color = Color.white;
            else timeText.color = textColor[text];
        }
    }
}
