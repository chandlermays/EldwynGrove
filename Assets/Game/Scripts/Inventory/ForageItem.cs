using UnityEngine;
//---------------------------------

namespace EldwynGrove.Inventory
{
    [CreateAssetMenu(fileName = "New Forage Item", menuName = "Eldwyn Grove/Inventory/Forage Item")]
    public class ForageItem : ScriptableObject
    {
        [SerializeField] private string m_itemName;
        [SerializeField] private Sprite m_itemIcon;

        [Tooltip("How many of this item is added to the inventory on gather.")]
        [SerializeField] private int m_forageYield = 1;

        public string ItemName => m_itemName;
        public Sprite Icon => m_itemIcon;
        public int ForageYield => m_forageYield;
    }
}