using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
//---------------------------------
using EldwynGrove.Saving;

namespace EldwynGrove.Inventories
{
    public class Wallet : MonoBehaviour, ISaveable
    {
        private int m_currentGold = 0;

        public event Action OnWalletUpdated;

        public int CurrentGold => m_currentGold;

        /*--------------------------------------------------------------------------------
        | --- UpdateSiver: Updates the current silver amount by the specified amount --- |
        --------------------------------------------------------------------------------*/
        public void UpdateSiver(int amount)
        {
            m_currentGold += amount;
            OnWalletUpdated?.Invoke();
        }

        /*----------------------------------------------------------------
        | --- CaptureState: Captures the current state of the wallet --- |
        ----------------------------------------------------------------*/
        public JToken CaptureState()
        {
            return JToken.FromObject(m_currentGold);
        }

        /*---------------------------------------------------------------------
        | --- RestoreState: Restores the wallet state from the saved data --- |
        ---------------------------------------------------------------------*/
        public void RestoreState(JToken state)
        {
            m_currentGold = state.ToObject<int>();
            OnWalletUpdated?.Invoke();
        }
    }
}