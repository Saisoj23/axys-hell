using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    bool _testMode = true;
    string _gameId = "3736897";
    string rewader = "rewardedVideo";
    string video = "video";

    UIController ui;
    GameController game;

    void Awake()
    {
        InitializeAds();
    }

    void Start()
    {
        ui = GetComponent<UIController>();
        game = GetComponent<GameController>();
    }
 
    public void InitializeAds()
    {
        #if UNITY_ANDROID
            _testMode = false;
        #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
        if (Advertisement.isInitialized)
        {
            LoadAd(0);
            LoadAd(1);
        }
    }

    public void LoadAd(int ad)
    {
        #if UNITY_ANDROID || UNITY_EDITOR
        if (ad == 0 && Advertisement.isInitialized && !Advertisement.isShowing) Advertisement.Load(video, this);
        else if (ad == 1 && Advertisement.isInitialized && !Advertisement.isShowing) Advertisement.Load(rewader, this);
        #else
        return;
        #endif
    }

    public void PlayAd(int ad)
    {
        #if UNITY_ANDROID || UNITY_EDITOR
        if (ad == 0 && Advertisement.isInitialized && !Advertisement.isShowing) Advertisement.Show(video, this);
        else if (ad == 1 && Advertisement.isInitialized && !Advertisement.isShowing) Advertisement.Show(rewader, this);
        #else
        return;
        #endif
    }

    public void OnUnityAdsShowComplete (string placementId, UnityAdsShowCompletionState showResult) 
    {
        if (showResult.Equals(UnityAdsShowCompletionState.COMPLETED) && placementId.Equals(rewader)) 
        {
            ui.SetColor(1, 1);
            game.savedScore = (int)game.score;
            PlayerPrefs.SetInt(game.difficulty + "SavedScore" + game.level, game.savedScore);
            ui.AppartButton();
            LoadAd(1);
        } 
        else if (showResult.Equals(UnityAdsShowCompletionState.SKIPPED) && placementId.Equals(rewader)) 
        {
            LoadAd(1);
            ui.AppartButton();

        } 
        else if (showResult.Equals(UnityAdsShowCompletionState.UNKNOWN) && placementId.Equals(rewader)) 
        {
            LoadAd(1);
            ui.SetColor(1, 1);
            game.savedScore = (int)game.score;
            PlayerPrefs.SetInt(game.difficulty + "SavedScore" + game.level, game.savedScore);
            ui.AppartButton();
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
        if (placementId.Equals(video)) LoadAd(0);
    }

    public void OnInitializationComplete()
    {
        LoadAd(0);
        LoadAd(1);
        Debug.Log("Unity Ads initialization complete.");
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }
 
    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }
 
    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }
}
