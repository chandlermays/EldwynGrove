using UnityEngine;
using UnityEngine.UI;
//---------------------------------
using EldwynGrove.Attributes;
using EldwynGrove.Components;

namespace EldwynGrove.UI
{
    public class EnergyBar : MonoBehaviour
    {
        [Header("Energy Bar Settings")]
        [SerializeField] private EnergyComponent m_energyComponent;
        private Image m_energyBarFill;

        private BaseStats m_baseStats;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            m_energyBarFill = GetComponent<Image>();
            Utilities.CheckForNull(m_energyBarFill, nameof(m_energyBarFill));
            Utilities.CheckForNull(m_energyComponent, nameof(m_energyComponent));

            m_baseStats = m_energyComponent.GetComponent<BaseStats>();
            Utilities.CheckForNull(m_baseStats, nameof(m_baseStats));
        }

        /*---------------------------------------------------------------------
        | --- OnEnable: Called when this object becomes enabled or active --- |
        ---------------------------------------------------------------------*/
        private void OnEnable()
        {
            m_energyComponent.OnEnergyChanged += UpdateEnergyBar;
        }

        /*---------------------------------------------------------------------------
        | --- OnDisable: Called when the behaviour becomes disabled or inactive --- |
        ---------------------------------------------------------------------------*/
        private void OnDisable()
        {
            m_energyComponent.OnEnergyChanged -= UpdateEnergyBar;
        }

        /*-------------------------------------------------------------------------------------
        | --- UpdateEnergyBar: Adjust the Fill Amount to the Current Energy of the Entity --- |
        -------------------------------------------------------------------------------------*/
        private void UpdateEnergyBar()
        {
            float maxEnergy = m_baseStats.GetEnergy();
            if (maxEnergy <= 0f)
                return;

            float healthPercentage = Mathf.Clamp(m_energyComponent.CurrentEnergy / maxEnergy, 0f, 1f);
            m_energyBarFill.fillAmount = healthPercentage;
        }
    }
}