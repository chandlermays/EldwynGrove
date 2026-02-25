using UnityEngine;
//---------------------------------

namespace EldwynGrove.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder
    {
        [SerializeField] private InventoryItemIcon m_itemIcon;

        private int m_index;
        private Inventory m_playerInventory;

        public InventoryItem GetItem() => m_playerInventory.GetItemAtSlot(m_index);
        public int GetQuantity() => m_playerInventory.GetQuantityAtSlot(m_index);

        /*---------------------------------------------------------------------------
        | --- Setup: Initialize the InventorySlotUI with an Inventory and index --- |
        ---------------------------------------------------------------------------*/
        public void Setup(Inventory inventory, int index)
        {
            m_playerInventory = inventory;
            m_index = index;

            m_itemIcon.SetItem(m_playerInventory.GetItemAtSlot(m_index), m_playerInventory.GetQuantityAtSlot(index));
        }

        /*-------------------------------------------------------------------------------------
        | --- GetMaxQuantity: Determine the maximum quantity of an item that can be added --- |
        -------------------------------------------------------------------------------------*/
        public int GetMaxQuantity(InventoryItem item)
        {
            if (m_playerInventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        /*-----------------------------------------------------------------------------
        | --- AddItems: Add a specified quantity of an item to the inventory slot --- |
        -----------------------------------------------------------------------------*/
        public void AddItems(InventoryItem item, int quantity)
        {
            m_playerInventory.TryAddItemToSlot(m_index, item, quantity);
        }

        /*-------------------------------------------------------------------------------------
        | --- RemoveItems: Remove a specified quantity of an item from the inventory slot --- |
        -------------------------------------------------------------------------------------*/
        public void RemoveItems(int quantity)
        {
            m_playerInventory.RemoveItemsFromSlot(m_index, quantity);
        }

        /*--------------------------------------------------------------------------------------
        | --- GetMaxItemsCapacity: Determine the maximum number of items that can be added --- |
        --------------------------------------------------------------------------------------*/
        public int GetMaxItemsCapacity(InventoryItem item)
        {
            if (m_playerInventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }
    }
}