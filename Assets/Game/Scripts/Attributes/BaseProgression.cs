/*-------------------------
File: BaseProgression.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Attributes
{
    public abstract class BaseProgression : ScriptableObject
    {
        [SerializeField] private float m_healthIncreasePercentage;
        [SerializeField] private int m_initialHealth;
        [SerializeField] private int[] m_maxHealthAmounts;

        [SerializeField] private float m_damageIncreasePercentage;
        [SerializeField] private int m_initialDamage;
        [SerializeField] private int[] m_damageAmounts;

        [SerializeField] private float m_defenseIncreasePercentage;
        [SerializeField] private int m_initialDefense;
        [SerializeField] private int[] m_defenseAmounts;

        [SerializeField] private float m_energyIncreasePercentage;
        [SerializeField] private int m_initialEnergy;
        [SerializeField] private int[] m_energyAmounts;

        [SerializeField] private float m_energyRegenIncreasePercentage;
        [SerializeField] private float m_initialEnergyRegenRate;
        [SerializeField] private float[] m_energyRegenRates;

        /*------------------------------------------------------------------------ 
        | --- OnEnable: Called when the Object becomes Enabled and is Active --- |
        ------------------------------------------------------------------------*/
        protected virtual void OnEnable()
        {
            if (m_maxHealthAmounts == null || m_maxHealthAmounts.Length == 0)
            {
                m_maxHealthAmounts = new int[1] { m_initialHealth };
            }

            if (m_damageAmounts == null || m_damageAmounts.Length == 0)
            {
                m_damageAmounts = new int[1] { m_initialDamage };
            }

            if (m_defenseAmounts == null || m_defenseAmounts.Length == 0)
            {
                m_defenseAmounts = new int[1] { m_initialDefense };
            }

            if (m_energyAmounts == null || m_energyAmounts.Length == 0)
            {
                m_energyAmounts = new int[1] { m_initialEnergy };
            }

            if (m_energyRegenRates == null || m_energyRegenRates.Length == 0)
            {
                m_energyRegenRates = new float[1] { m_initialEnergyRegenRate };
            }
        }

        /*----------------------------------------------------------------------------------------------- 
        | --- RecalculateMaxHealthAmounts: Recalculate the Health Amounts for the Class Progression --- |
        -----------------------------------------------------------------------------------------------*/
        public void RecalculateMaxHealthAmounts()
        {
            if (m_maxHealthAmounts.Length == 0)
                return;

            m_maxHealthAmounts[0] = m_initialHealth;
            for (int i = 1; i < m_maxHealthAmounts.Length; ++i)
            {
                m_maxHealthAmounts[i] = Mathf.RoundToInt(m_maxHealthAmounts[i - 1] * (1 + m_healthIncreasePercentage / 100f));
            }
        }

        /*-------------------------------------------------------------------------------------------- 
        | --- RecalculateDamageAmounts: Recalculate the Damage Amounts for the Class Progression --- |
        --------------------------------------------------------------------------------------------*/
        public void RecalculateDamageAmounts()
        {
            if (m_damageAmounts.Length == 0)
                return;

            m_damageAmounts[0] = m_initialDamage;
            for (int i = 1; i < m_damageAmounts.Length; ++i)
            {
                m_damageAmounts[i] = Mathf.RoundToInt(m_damageAmounts[i - 1] * (1 + m_damageIncreasePercentage / 100f));
            }
        }

        /*---------------------------------------------------------------------------------------------- 
        | --- RecalculateDefenseAmounts: Recalculate the Defense Amounts for the Class Progression --- |
        ----------------------------------------------------------------------------------------------*/
        public void RecalculateDefenseAmounts()
        {
            if (m_defenseAmounts.Length == 0)
                return;

            m_defenseAmounts[0] = m_initialDefense;
            for (int i = 1; i < m_defenseAmounts.Length; ++i)
            {
                m_defenseAmounts[i] = Mathf.RoundToInt(m_defenseAmounts[i - 1] * (1 + m_defenseIncreasePercentage / 100f));
            }
        }

        /*----------------------------------------------------------------------------------------------- 
        | --- RecalculateMaxEnergyAmounts: Recalculate the Energy Amounts for the Class Progression --- |
        -----------------------------------------------------------------------------------------------*/
        public void RecalculateMaxEnergyAmounts()
        {
            if (m_energyAmounts.Length == 0)
                return;

            m_energyAmounts[0] = m_initialEnergy;
            for (int i = 1; i < m_energyAmounts.Length; ++i)
            {
                m_energyAmounts[i] = Mathf.RoundToInt(m_energyAmounts[i - 1] * (1 + m_energyIncreasePercentage / 100f));
            }
        }

        /*--------------------------------------------------------------------------------------------------- 
        | --- RecalculateEnergyRegenRates: Recalculate the Energy Regen Rates for the Class Progression --- |
        ---------------------------------------------------------------------------------------------------*/
        public void RecalculateEnergyRegenRates()
        {
            if (m_energyRegenRates.Length == 0)
                return;

            m_energyRegenRates[0] = m_initialEnergyRegenRate;
            for (int i = 1; i < m_energyRegenRates.Length; ++i)
            {
                m_energyRegenRates[i] = m_energyRegenRates[i - 1] * (1 + m_energyRegenIncreasePercentage / 100f);
            }
        }

        /*---------------------------------------------------------------------- 
        | --- GetHealth: Returns the Health Amount for the Specified Level --- |
        ----------------------------------------------------------------------*/
        public float GetHealth(int level)
        {
            if (level < 1 || level > m_maxHealthAmounts.Length)
            {
                Debug.LogWarning("Level out of range");
                return 0;
            }

            return m_maxHealthAmounts[level - 1];
        }

        /*---------------------------------------------------------------------- 
        | --- GetDamage: Returns the Damage Amount for the Specified Level --- |
        ----------------------------------------------------------------------*/
        public float GetDamage(int level)
        {
            if (level < 1 || level > m_damageAmounts.Length)
            {
                Debug.LogWarning("Level out of range");
                return 0;
            }

            return m_damageAmounts[level - 1];
        }

        /*------------------------------------------------------------------------ 
        | --- GetDefense: Returns the Defense Amount for the Specified Level --- |
        ------------------------------------------------------------------------*/
        public float GetDefense(int level)
        {
            if (level < 1 || level > m_defenseAmounts.Length)
            {
                Debug.LogWarning("Level out of range");
                return 0;
            }

            return m_defenseAmounts[level - 1];
        }

        /*---------------------------------------------------------------------- 
        | --- GetEnergy: Returns the Energy Amount for the Specified Level --- |
        ----------------------------------------------------------------------*/
        public float GetEnergy(int level)
        {
            if (level < 1 || level > m_energyAmounts.Length)
            {
                Debug.LogWarning("Level out of range");
                return 0;
            }

            return m_energyAmounts[level - 1];
        }

        /*----------------------------------------------------------------------------------- 
        | --- GetEnergyRegenRate: Returns the Energy Regen Rate for the Specified Level --- |
        -----------------------------------------------------------------------------------*/
        public float GetEnergyRegenRate(int level)
        {
            if (level < 1 || level > m_energyRegenRates.Length)
            {
                Debug.LogWarning("Level out of range");
                return 0;
            }

            return m_energyRegenRates[level - 1];
        }
    }
}