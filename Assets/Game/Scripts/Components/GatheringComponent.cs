/*-------------------------
File: GatheringComponent.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Inventories;

namespace EldwynGrove.Components
{
    public class GatheringComponent : EntityComponent
    {
        private const float kGatherEnergyCost = 1f;

        private Equipment m_equipment;
        private Inventory m_inventory;
        private EnergyComponent m_energyComponent;

        private static readonly int s_animChop = Animator.StringToHash("Chop");
        private static readonly int s_animMine = Animator.StringToHash("Mine");
        private static readonly int s_animReap = Animator.StringToHash("Reap");

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        protected override void Awake()
        {
            base.Awake();

            m_inventory = GetComponent<Inventory>();
            Utilities.CheckForNull(m_inventory, nameof(m_inventory));

            m_equipment = GetComponent<Equipment>();
            Utilities.CheckForNull(m_equipment, nameof(m_equipment));

            m_energyComponent = GetComponent<EnergyComponent>();
            Utilities.CheckForNull(m_energyComponent, nameof(m_energyComponent));
        }

        /*-------------------------------------------------------------------------------------------------------
        | --- Gather: Triggers the appropriate gathering animation based on the type of item being gathered --- |
        -------------------------------------------------------------------------------------------------------*/
        public bool Gather(ForageItem item)
        {
            if (item == null)
                return false;

            EquipmentSlot requiredSlot = GetRequiredSlot(item.GatherType);
            if (requiredSlot != EquipmentSlot.kNone)
            {
                EquipableItem equipped = m_equipment.GetItemInSlot(requiredSlot);
                if (equipped == null || equipped is not ToolItem)
                {
                    // Replace with in-game UI prompt
                    Debug.LogWarning($"[GatheringComponent] Missing required tool for {item.GatherType}.");
                    return false;
                }
            }

            if (!m_energyComponent.UseEnergy(kGatherEnergyCost))
            {
                // Replace with in-game UI prompt
                Debug.LogWarning("[GatheringComponent] Not enough energy to gather.");
                return false;
            }

            int trigger = item.GatherType switch
            {
                GatherType.kChop => s_animChop,
                GatherType.kMine => s_animMine,
                GatherType.kReap => s_animReap,
                _ => s_animReap
            };

            Animator.SetTrigger(trigger);

            bool slotAvailable = m_inventory.TryAddToAvailableSlot(item, item.ForageYield);
            if (!slotAvailable)
            {
                // Replace with in-game UI prompt
                Debug.LogWarning("[GatheringComponent] No available inventory slot to add the gathered item.");
                return false;
            }

            return true;
        }

        /*-----------------------------------------------------------------------------------------
        | --- GetRequiredSlot: Determines the required equipment slot for a given gather type --- |
        -----------------------------------------------------------------------------------------*/
        private static EquipmentSlot GetRequiredSlot(GatherType gatherType) => gatherType switch
        {
            GatherType.kChop => EquipmentSlot.kAxe,
            GatherType.kMine => EquipmentSlot.kPickaxe,
            GatherType.kReap => EquipmentSlot.kHoe,
            _ => EquipmentSlot.kNone
        };
    }
}