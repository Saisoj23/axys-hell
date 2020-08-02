using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScreenController : MonoBehaviour
{
    [Header("Narrative")]
    [TextArea]
    public string[] normalModeIntro;
    [TextArea]
    public string[] normalModeEnding;
    [TextArea]
    public string[] hardModeIntro;
    [TextArea]
    public string[] hardModeEnding;
    [TextArea]
    public string[] altNomalModeIntro;
    [TextArea]
    public string[] altNormalModeEnding;
    [TextArea]
    public string[] altHardModeIntro;

    [Header("Credits")]
    [TextArea]
    public string[] creditsText;

    string[] textToShow;

    public TMPro.TMP_Text text;
    public TMPro.TMP_Text credits;
    public AudioSource sfx;

    public float writingSpeed;
    public float creditsSpeed;
    public float creditsWait;
    
    MusicAndData musicAndData;
    SceneChange scene;

    bool skip = false;
    bool playing = false;
    bool exit = false;

    int difficulty = 0;
    int progress = 0;

    int textIndex = 0;
    string lastText = "";

    void Awake()
    {
        musicAndData = FindObjectOfType<MusicAndData>();
        scene = FindObjectOfType<SceneChange>();

        if (musicAndData != null) musicAndData.PlayMusic(true);

        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        progress = PlayerPrefs.GetInt(difficulty + "Progress", 0);

        //normal mode
        if (difficulty == 0)
        {
            //intro
            if (progress == 0)
            {
                //def
                if (PlayerPrefs.GetInt(1 + "Progress", 0) != 2) textToShow = normalModeIntro;
                //alt
                else textToShow = altNomalModeIntro;
            }
            //ending
            else if (progress == 1)
            {
                //def
                if (PlayerPrefs.GetInt(1 + "Progress", 0) != 2) textToShow = normalModeEnding;
                else textToShow = altNormalModeEnding;
            }
        }
        //hard mode
        else
        {
            //intro
            if (progress == 0)
            {
                if (PlayerPrefs.GetInt(0 + "Progress", 0) == 2) textToShow = hardModeIntro;
                else textToShow = altHardModeIntro;
            }
            else if (progress == 1)
            {
                textToShow = hardModeEnding;
            }
        }

        if (progress == 0)
            StartCoroutine(ShowText());
        else if (progress == 1) StartCoroutine(ShowCredits());
    }

    public void PlayClicked ()
    {
        if (!playing && textIndex < textToShow.Length) StartCoroutine(ShowText());
        else if (!playing && textIndex >= textToShow.Length)
        {
            progress++;
            PlayerPrefs.SetInt(difficulty + "Progress", progress);
            if (progress == 1) scene.ChangeScene("level_normal");
            else if (progress == 2) scene.Exit();
        }
        else if (playing) skip = true;
    }

    IEnumerator ShowText ()
    {
        playing = true;
        for (int i = 0; i < textToShow[textIndex].Length; i++)
        {
            if (skip)
            {
                skip = false;
                i = textToShow[textIndex].Length - 1;
            }
            text.text = lastText + textToShow[textIndex].Substring(0, i + 1);
            if (!sfx.isPlaying) sfx.Play();
            yield return new WaitForSeconds(writingSpeed);
        }
        text.text += "\n";
        lastText = text.text;
        textIndex++;

        playing = false;
    }

    IEnumerator ShowCredits ()
    {
        playing = true;
        if (musicAndData != null) musicAndData.PlayMusic(true);
        for (int i = 0; i < creditsText.Length; i++)
        {
            credits.text = creditsText[i];

            for (float j = 0; j < 1; j += Time.deltaTime * creditsSpeed)
            {
                credits.color = Color.Lerp(new Color(1f,1f,1f,0f), new Color(1f,1f,1f,1f), j);
                yield return null;
            }
            credits.color = new Color(1f,1f,1f,1f);

            yield return new WaitForSeconds(creditsWait);
            for (float j = 0; j < 1; j += Time.deltaTime * creditsSpeed)
            {
                credits.color = Color.Lerp(new Color(1f,1f,1f,1f), new Color(1f,1f,1f,0f), j);
                yield return null;
            }
            credits.color = new Color(1f,1f,1f,0f);

            yield return new WaitForSeconds(1f);
        }
        if (musicAndData != null) musicAndData.PlayMusic(false);
        yield return new WaitForSeconds(3f);
        StartCoroutine(ShowText());
    }
}
