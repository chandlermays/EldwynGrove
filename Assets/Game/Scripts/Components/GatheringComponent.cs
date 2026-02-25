using UnityEngine;
//---------------------------------
using EldwynGrove.Inventories;

namespace EldwynGrove
{
    public class GatheringComponent : EntityComponent
    {
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
    }
}