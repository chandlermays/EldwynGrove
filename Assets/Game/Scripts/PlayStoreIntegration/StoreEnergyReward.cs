/*-------------------------
File: StoreEnergyReward.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Components;

public class StoreEnergyReward : MonoBehaviour
{
    [SerializeField] private float m_energyRewardAmount = 50f;
    [SerializeField] private EnergyComponent m_energyComponent;

    public void WatchAdForEnergy()
    {
        StoreAdsManager.Instance.AddRewardAction(OnAdRewarded);
        StoreAdsManager.Instance.PlayRewardedAd();
    }

    private void OnAdRewarded()
    {
        if (m_energyComponent != null)
        {
            m_energyComponent.ReplenishEnergy(m_energyRewardAmount);
            Debug.Log($"Rewarded {m_energyRewardAmount} energy after ad.");
        }
    }
}