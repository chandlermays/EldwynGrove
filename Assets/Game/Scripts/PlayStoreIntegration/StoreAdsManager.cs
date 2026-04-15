using UnityEngine;
using System.Collections.Generic;

using UnityEngine.Advertisements;
using System;

// Travis Torres-O'Callaghan
// 4/11/26

// Store Ads Manager
//
// Initializes ads and makes them accessible to other classes.
//
// Other classes can run ads with this class,
// and for ad types like Rewarded Ads, receive a bool for if completed

//
//What to know if you're implementing ads with this class:
//
// 1. To add rewards, add Actions (w/ no params) using AddRewardAction()
// 2. To play an ad, call PlayRewardedAd()
//
// 3. When calling PlayRewardedAd(), the following will happen:
//         - Advertisements attempts to load an ad
//         - If ad is loaded, it is played for user
//         - If ad is not loaded or quit out early, all callbacks are removed so player gets no rewards.
//         - Finally, if player watches through to the end of ad, every callback will happen in order
//


public class StoreAdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    //Private Singleton Instance
    private static StoreAdsManager m_instance;

    //Singleton Getter Property
    static public StoreAdsManager Instance => m_instance;

    //Id that is used for rewarded ads
    [SerializeField] private string m_androidId = "5716486"; //test id
    [SerializeField] private string m_rewardedAdId = ""; //test id
    
    //were ads successfully initialized?
    [SerializeField] private bool m_successfullyInitializedAds = false;
    [SerializeField] bool m_loadedAd = false; //did we load ad yet?

    //Functions to call on ad success - gets cleared on ad failure
    private List<System.Action> m_onAdCompletedCallbacks = new List<System.Action>();

    ///////////////////////
    //Awake() - Handle singleton
    ///////////////////////
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /////////////////////////
    //Start() - Initialize Advertisements
    /////////////////////////
    
    private void Start()
    {
#if UNITY_ANDROID
        Debug.Log("Android build is active.");
#if ADS_ENABLED
            Debug.Log("Ads are active.");
    
        //developer mode 
#if TEST_BUILD
            //initialze ads
            bool testMode = true;
            Advertisement.Initialize(m_androidId, testMode, this);
#else
            //initialze ads
            bool testMode = false;
            //initialze ads
            Advertisement.Initialize(m_androidId, testMode, this);

#endif

#endif
#else
        Debug.Log("Android build is not active.");
#endif
    }

    ///////////////////////
    //PlayRewardedAd() - attempt to play a rewarded ad
    //                   (plr will be given rewards if ad is finished)
    ///////////////////////

    public void PlayRewardedAd()
    {
        //If ads are enabled, try to play ad
#if ADS_ENABLED

        //make sure an ad is loaded
        LoadAd();
#else
        //if ads are disabled, always fail to play ad
        Debug.Log("Ads Disabled. Could not play ad.");
        return;
#endif
    }



    /////////////////////////
    //OnInitializationComplete()
    /////////////////////////
    
    public void OnInitializationComplete()
    {
        m_successfullyInitializedAds = true;
        Debug.Log("Successfully initialized ads!");
    }

    /////////////////////////
    //OnInitializationFailed()
    /////////////////////////

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        m_successfullyInitializedAds = false;
        Debug.LogError(message + " Error: " + error);
    }


    /////////////////////////
    //LoadAd() - Get Advertisement to start loading an ad
    /////////////////////////
    
    private void LoadAd()
    {
#if ADS_ENABLED
        Debug.Log("Loading ad...");

        if (m_loadedAd == false)
        {
            Advertisement.Load(m_rewardedAdId, this);
            m_loadedAd = true;
        }
#endif
    }

    /////////////////////////
    //OnUnityAdsAdLoaded() - show ad on suuccessful ad loaded
    /////////////////////////
    
    public void OnUnityAdsAdLoaded(string placementId)
    {
#if ADS_ENABLED
        if (placementId.Equals(m_rewardedAdId))
        {
            Debug.Log("Playing ad!");
            Advertisement.Show(m_rewardedAdId, this);
            m_loadedAd = false;
        }

#endif
    }

    /////////////////////////
    //OnUnityAdsAdLoaded() - show ad on suuccessful ad loaded
    /////////////////////////
    
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
#if ADS_ENABLED
        Debug.LogWarning("Error loading Ad Unit :" + error.ToString() + " " + message);
        m_loadedAd = false;
#endif
    }

    ////////////////////////////////
    //AddRewardAction(callback) - add callback to list of callbacks
    ////////////////////////////////
    
    public void AddRewardAction(Action callback)
    {
#if ADS_ENABLED
        m_onAdCompletedCallbacks.Add(callback);

        if (m_loadedAd == false)
        {
            Advertisement.Load(m_rewardedAdId, this);
            m_loadedAd = true;
        }
#endif
    }

    /////////////////////////
    //OnUnityAdsShowFailure() - remove all reward callbacks
    /////////////////////////

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        m_onAdCompletedCallbacks.Clear();
    }

    /////////////////////////
    //OnUnityAdsShowStart()
    /////////////////////////
    public void OnUnityAdsShowStart(string placementId) {  }

    /////////////////////////
    //OnUnityAdsShowClick()
    /////////////////////////
    public void OnUnityAdsShowClick(string placementId) {  }

    /////////////////////////
    //OnUnityAdsShowComplete() - 
    /////////////////////////

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
#if ADS_ENABLED
        //if ad was completed
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Ad fully completed!");

            //loop through and call each callback
            for (int i = 0; i < m_onAdCompletedCallbacks.Count; i++)
            {
                m_onAdCompletedCallbacks[i].Invoke();
            }
        }
        else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
        {
            Debug.Log("Ad skipped");
        }
        else
        {
            Debug.LogWarning("Ad completion state unknown.");
        }

        //remove all action callbacks
        m_onAdCompletedCallbacks.Clear();
#endif
    }

}
