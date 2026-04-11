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
        private Equipment m_equipment;
        private Inventory m_inventory;

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
        }

        /*-------------------------------------------------------------------------------------------------------
        | --- Gather: Triggers the appropriate gathering animation based on the type of item being gathered --- |
        -------------------------------------------------------------------------------------------------------*/
        public void Gather(ForageItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("[GatheringComponent] Attempted to gather with a null item.");
                return;
            }

            EquipmentSlot requiredSlot = GetRequiredSlot(item.GatherType);
            if (requiredSlot != EquipmentSlot.kNone)
            {
                EquipableItem equipped = m_equipment.GetItemInSlot(requiredSlot);
                if (equipped == null || equipped is not ToolItem)
                {
                    Debug.LogWarning($"[GatheringComponent] Missing required tool for {item.GatherType}.");
                    return;
                }
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
                Debug.LogWarning("[GatheringComponent] No available inventory slot to add the gathered item.");
            }
        }

        private static EquipmentSlot GetRequiredSlot(GatherType gatherType) => gatherType switch
        {
            GatherType.kChop => EquipmentSlot.kAxe,
            GatherType.kMine => EquipmentSlot.kPickaxe,
            GatherType.kReap => EquipmentSlot.kHoe,
            _ => EquipmentSlot.kNone
        };
    }
}