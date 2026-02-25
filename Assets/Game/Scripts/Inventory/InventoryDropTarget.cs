using EldwynGrove.UI.Dragging;
using UnityEngine;

namespace EldwynGrove.Inventories
{
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        [SerializeField] private GameObject m_player;

        private ItemDropper m_itemDropper;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_player, nameof(m_player));

            m_itemDropper = m_player.GetComponent<ItemDropper>();
            Utilities.CheckForNull(m_itemDropper, nameof(m_itemDropper));
        }

        /*---------------------------------------------------------------------------------------------
        | --- AddItems: Drops the specified item(s) into the world, "adding" to the "destination" --- |
        ---------------------------------------------------------------------------------------------*/
        public void AddItems(InventoryItem item, int quantity)
        {
            m_itemDropper.DropItem(item, quantity);
        }

        /*--------------------------------------------------------------------------------------
        | --- GetMaxItemsCapacity: Determine the maximum number of items that can be added --- |
        --------------------------------------------------------------------------------------*/
        public int GetMaxItemsCapacity(InventoryItem item)
        {
            return int.MaxValue;
        }
    }
}