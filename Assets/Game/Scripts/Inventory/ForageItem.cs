using UnityEngine;
//---------------------------------

namespace EldwynGrove.Inventories
{
    public enum GatherType
    {
        kChop,
        kMine,
        kReap
    }

    [CreateAssetMenu(fileName = "New Forage Item", menuName = "Eldwyn Grove/Inventory/Forage Item")]
    public class ForageItem : ScriptableObject
    {
        [SerializeField] private string m_itemName;
        [SerializeField] private Sprite m_itemIcon;

        [Tooltip("How many of this item is added to the inventory on gather.")]
        [SerializeField] private int m_forageYield = 1;

        [Tooltip("The type of gathering action required to obtain this item.")]
        [SerializeField] private GatherType m_gatherType;

        public string ItemName => m_itemName;
        public Sprite Icon => m_itemIcon;
        public int ForageYield => m_forageYield;
        public GatherType GatherType => m_gatherType;
    }
}