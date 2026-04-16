/*-------------------------
File: StoreEnergyReward.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Components;

namespace EldwynGrove.Ads
{
    public class StoreEnergyReward : MonoBehaviour
    {
        [SerializeField] private float m_energyRewardAmount = 50f;
        [SerializeField] private EnergyComponent m_energyComponent;

        /*-----------------------------------------------------------------------------------------------
        | --- WatchAdForEnergy: Called when the player opts to watch an ad for energy replenishment --- |
        -----------------------------------------------------------------------------------------------*/
        public void WatchAdForEnergy()
        {
            StoreAdsManager.Instance.AddRewardAction(OnAdRewarded);
            StoreAdsManager.Instance.PlayRewardedAd();
        }

        /*----------------------------------------------------------------------------------------------
        | --- OnAdRewarded: Called when the rewarded ad finishes successfully, replenishing energy --- |
        ----------------------------------------------------------------------------------------------*/
        private void OnAdRewarded()
        {
            if (m_energyComponent != null)
            {
                m_energyComponent.ReplenishEnergy(m_energyRewardAmount);
                Debug.Log($"Rewarded {m_energyRewardAmount} energy after ad.");
            }
        }
    }
}