/*-------------------------
File: ObtainItemEvent.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Inventories;
using EldwynGrove.Saving;

namespace EldwynGrove.Dialogues
{
    [CreateAssetMenu(menuName = "Eldwyn Grove/Dialogue/Events/Obtain Item", fileName = "New Obtain Item Event")]
    public class ObtainItemEvent : DialogueEvent
    {
        [SerializeField] private InventoryItem m_item;
        [SerializeField] private int m_quantity = 1;

        /*-----------------------------------------------------------------------
        | --- Execute: Gives the configured item to the player (instigator) --- |
        -----------------------------------------------------------------------*/
        public override void Execute(GameObject instigator, GameObject target)
        {
            if (m_item == null)
                return;

            Inventory inventory = instigator.GetComponent<Inventory>();
            ItemDropper dropper = instigator.GetComponent<ItemDropper>();

            if (m_item.IsStackable)
            {
                bool success = inventory.TryAddToAvailableSlot(m_item, m_quantity);
                if (!success)
                {
                    dropper.DropItem(m_item, m_quantity);
                }
            }
            else
            {
                for (int i = 0; i < m_quantity; ++i)
                {
                    bool success = inventory.TryAddToAvailableSlot(m_item, 1);
                    if (!success)
                    {
                        dropper.DropItem(m_item);
                    }
                }
            }

            SaveManager.Instance.Save();
        }
    }
}