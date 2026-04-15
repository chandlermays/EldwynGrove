using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
//---------------------------------
using EldwynGrove.Attributes;
using EldwynGrove.Saving;

namespace EldwynGrove.Components
{
    public class EnergyComponent : EntityComponent, ISaveable
    {
        [Header("Energy Settings")]
        [SerializeField] private float m_energy;
        [SerializeField] private float m_energyRegenRate;

        private float m_maxEnergy;
        public event Action OnEnergyChanged;

        public float CurrentEnergy => m_energy;
        public float EnergyRegenRate => m_energyRegenRate;

        private BaseStats m_stats;

        private bool m_hasBeenInitialized = false;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        protected override void Awake()
        {
            base.Awake();

            m_stats = GetComponent<BaseStats>();
            Utilities.CheckForNull(m_stats, nameof(BaseStats));
        }

        /*-----------------------------------------------------
        | --- Start: Called before the first frame update --- |
        -----------------------------------------------------*/
        private void Start()
        {
            if (!m_hasBeenInitialized)
            {
                m_maxEnergy = m_stats.GetEnergy();
                m_energyRegenRate = m_stats.GetEnergyRegenRate();
                m_energy = m_maxEnergy;
                m_hasBeenInitialized = true;
                OnEnergyChanged?.Invoke();
            }
        }

        /*---------------------------------------
        | --- Update: Called once per frame --- |
        ---------------------------------------*/
        private void Update()
        {
            if (m_energy < m_maxEnergy)
            {
                m_energy += m_energyRegenRate * Time.deltaTime;
                if (m_energy > m_maxEnergy)
                {
                    m_energy = m_maxEnergy;
                }
                OnEnergyChanged?.Invoke();
            }
        }

        /*-------------------------------------------------------------------------------
        | --- RecalculateEnergy: Recalculate the Entity's Max Energy and Regen Rate --- |
        -------------------------------------------------------------------------------*/
        public void RecalculateEnergy()
        {
            float newMaxEnergy = m_stats.GetEnergy();
            float newRegenRate = m_stats.GetEnergyRegenRate();

            // first time initialization safeguard
            if (m_maxEnergy <= 0f)
            {
                m_maxEnergy = newMaxEnergy;
                m_energyRegenRate = newRegenRate;
                m_energy = Mathf.Clamp(m_energy, 0f, newMaxEnergy);
                OnEnergyChanged?.Invoke();
                return;
            }

            if (m_energy > newMaxEnergy)
            {
                m_energy = newMaxEnergy;
            }

            m_maxEnergy = newMaxEnergy;
            m_energyRegenRate = newRegenRate;

            OnEnergyChanged?.Invoke();
        }

        /*------------------------------------------------------------------
        | --- UseEnergy: Attempt to Use the Specified Amount of Energy --- |
        ------------------------------------------------------------------*/
        public bool UseEnergy(float amount)
        {
            if (amount > m_energy)
                return false;

            m_energy -= amount;
            OnEnergyChanged?.Invoke();
            return true;
        }

        /*------------------------------------------------------------------
        | --- ReplenishEnergy: Replenish the Entity's Energy by amount --- |
        ------------------------------------------------------------------*/
        public void ReplenishEnergy(float amount)
        {
            m_energy = Mathf.Min(m_energy + amount, m_stats.GetEnergy());
            OnEnergyChanged?.Invoke();
        }

        /*-------------------------------------------------------------------
        | --- FullyReplenishEnergy: Fully replenish the Entity's Energy --- |
        -------------------------------------------------------------------*/
        public void FullyReplenishEnergy()
        {
            m_maxEnergy = m_stats.GetEnergy();
            m_energyRegenRate = m_stats.GetEnergyRegenRate();
            m_energy = m_maxEnergy;
            OnEnergyChanged?.Invoke();
        }

        /*-------------------------------------------------------------------------
        | --- CaptureState: Captures the current state of the Entity's Energy --- |
        -------------------------------------------------------------------------*/
        public JToken CaptureState()
        {
            return JToken.FromObject(m_energy);
        }

        /*--------------------------------------------------------------
        | --- RestoreState: Restores the state from the saved data --- |
        --------------------------------------------------------------*/
        public void RestoreState(JToken state)
        {
            m_energy = state.ToObject<float>();
            m_hasBeenInitialized = true;
            OnEnergyChanged?.Invoke();
        }
    }
}