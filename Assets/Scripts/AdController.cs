using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdController : MonoBehaviour, IUnityAdsListener
{
    string gameId = "3736897";
    string rewader = "rewardedVideo";
    string video = "video";

    UIController ui;
    GameController game;

    void Start()
    {
        ui = GetComponent<UIController>();
        game = GetComponent<GameController>();

        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, true);
    }

    public void PlayAd(int ad)
    {
        if (ad == 0 && Advertisement.isInitialized && !Advertisement.isShowing) Advertisement.Show(video);
        else if (ad == 1 && Advertisement.isInitialized && !Advertisement.isShowing) Advertisement.Show(rewader);
    }

    public void OnUnityAdsReady (string placementId) 
    {
        if (placementId == rewader) 
        {        
            //adButton[0].interactable = true;
            //adButton[1].interactable = true;
        }
    }

    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) 
    {
        if (showResult == ShowResult.Finished && placementId == rewader) 
        {
            ui.SetColor(1, 1);
            game.savedScore = (int)game.score;
            PlayerPrefs.SetInt(game.difficulty + "SavedScore" + game.level, game.savedScore);
            ui.AppartButton();
        } 
        else if (showResult == ShowResult.Skipped && placementId == rewader) 
        {
            ui.AppartButton();

        } 
        else if (showResult == ShowResult.Failed && placementId == rewader) 
        {
            ui.SetColor(1, 1);
            game.savedScore = (int)game.score;
            PlayerPrefs.SetInt(game.difficulty + "SavedScore" + game.level, game.savedScore);
            ui.AppartButton();
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidError (string message) 
    {
    }

    public void OnUnityAdsDidStart (string placementId)
    {
    } 
}
