using UnityEngine;
//---------------------------------
using EldwynGrove.Inventories;

namespace EldwynGrove.Pickups
{
    public class Pickup : MonoBehaviour
    {
        private Inventory m_inventory;
        private InventoryItem m_item;
        private int m_quantity = 1;

        public InventoryItem Item => m_item;
        public int Quantity => m_quantity;

        private const string kPlayerTag = "Player";

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            var player = GameObject.FindWithTag(kPlayerTag);
            m_inventory = player.GetComponent<Inventory>();
            Utilities.CheckForNull(m_inventory, nameof(m_inventory));
        }

        /*-----------------------------------------
        | --- Update: Called upon every frame --- |
        -----------------------------------------*/
        private void Update()
        {

        }

        /*----------------------------------------------------------------
        | --- Setup: Initialize the Pickup with an item and quantity --- |
        ----------------------------------------------------------------*/
        public void Setup(InventoryItem item, int quantity)
        {
            m_item = item;
            m_quantity = item.IsStackable ? quantity : 1;
        }

        /*--------------------------------------------------------------------------
        | --- PickupItem: Add the item to the inventory and destroy the Pickup --- |
        --------------------------------------------------------------------------*/
        public void PickupItem()
        {
            bool slotAvailable = m_inventory.TryAddToAvailableSlot(m_item, m_quantity);
            if (slotAvailable)
            {
                Destroy(gameObject);
            }
        }

        /*---------------------------------------------------------------------------------------------
        | --- CanBePickedUp: Check if there's space in the inventory for the item to be picked up --- |
        ---------------------------------------------------------------------------------------------*/
        public bool CanBePickedUp()
        {
            if (m_inventory == null)
            {
                Debug.Log("Pickup: Inventory is not assigned on the player.");
                return false;
            }
            else
            {
                return m_inventory.HasSpaceFor(m_item);
            }
        }
    }
}