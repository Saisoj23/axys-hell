using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomLevel : MonoBehaviour
{
    public int[] spanwsActive = {1, 0, 0, 0};
    public int musicActive;
    public int themeActive;

    public bool canEnter = true;

    string spawnsString = "";
    int difficulty;

    public Image[] spawnsImages;
    public Image[] musicsImages;
    public Image[] themesImages;

    public Button[] spawnsButtons;
    public Button[] musicsButtons;
    public Button[] themesButtons;

    public Image border;
    public Image disk;
    public Image cover;

    public Sprite[] borderSprites;
    public Sprite[] diskSprites;
    public Sprite[] coverSprites;

    public TMPro.TMP_Text bestScoresText;
    public TMPro.TMP_Text lastScoresText;

    public Color disableColor;

    void Awake()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        int completedLevels = PlayerPrefs.GetInt(difficulty + "CompletedLevels", -1);

        for (int i = 0; i < spanwsActive.Length; i++)
        {
            spanwsActive[i] = 0;
            spawnsImages[i].color = disableColor;
        }
        if (PlayerPrefs.GetInt(difficulty + "CustomSpawn" + 0, 1) == 1)
        {
            spawnsString += spanwsActive[0];
            SpawnsButton(0);
        }
        for (int i = 1; i < 4; i++)
        {
            spawnsString += spanwsActive[i];
            if (PlayerPrefs.GetInt(difficulty + "CustomSpawn" + i) == 1) SpawnsButton(i);
        }

        MusicButton(PlayerPrefs.GetInt(difficulty + "CustomMusic", 0));
        ThemeButton(PlayerPrefs.GetInt(difficulty + "CustomTheme", 0));

        for (int i = 0; i < spawnsButtons.Length; i++)
        {
            if (i > completedLevels)
            {
                spawnsButtons[i].interactable = false;
                musicsButtons[i].interactable = false;
                themesButtons[i].interactable = false;
            }
        }
        if (completedLevels < 1)
        {
            border.color = disableColor;
            disk.color = disableColor;
            cover.color = disableColor;
            canEnter = false;
        }
        else canEnter = true;
    }

    public void SpawnsButton (int button)
    {
        if (spanwsActive[button] == 1)
        {
            int values = 0;
            for (int i = 0; i < spanwsActive.Length; i++)
            {
                if (i != button) values += spanwsActive[i];
            }
            if (values == 0) return;
        }

        spanwsActive[button] = spanwsActive[button] == 1 ? 0 : 1;
        
        spawnsString = "";
        for (int i = 0; i < spanwsActive.Length; i++)
        {
            spawnsString += spanwsActive[i];
        }

        spawnsImages[button].color = spanwsActive[button] == 1 ? Color.white : disableColor;
        int lastActive = 0;
        for (int i = 0; i < spanwsActive.Length; i++)
        {
            if (spanwsActive[i] == 1) lastActive = i;
        }
        border.sprite = borderSprites[lastActive];

        PlayerPrefs.SetInt(difficulty + "CustomSpawn" + button, spanwsActive[button]);
        
        bestScoresText.text = PlayerPrefs.GetInt(difficulty + "CustomBestScore" + spawnsString, 0).ToString();
        lastScoresText.text = PlayerPrefs.GetInt(difficulty + "CustomLastScore" + spawnsString, 0).ToString();
    }

    public void MusicButton (int button)
    {
        musicActive = button;

        for (int i = 0; i < musicsImages.Length; i++)
        {
            if (i != musicActive) musicsImages[i].color = disableColor;
        }
        musicsImages[musicActive].color = Color.white;
        disk.sprite = diskSprites[musicActive];

        PlayerPrefs.SetInt(difficulty + "CustomMusic", button);
    }

    public void ThemeButton (int button)
    {
        themeActive = button;

        for (int i = 0; i < themesImages.Length; i++)
        {
            if (i != themeActive) themesImages[i].color = disableColor;
        }
        themesImages[themeActive].color = Color.white;
        cover.sprite = coverSprites[themeActive];

        PlayerPrefs.SetInt(difficulty + "CustomTheme", button);
    }
}
