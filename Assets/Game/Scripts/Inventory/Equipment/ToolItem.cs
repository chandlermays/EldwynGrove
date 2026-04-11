/*-------------------------
File: ToolItem.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Inventories
{
    [CreateAssetMenu(fileName = "New Tool Item", menuName = "Eldwyn Grove/Items/Tool Item")]
    public class ToolItem : EquipableItem
    {
        [Min(1)]
        [SerializeField] private int m_maxDurability = 10;

        public int MaxDurability => m_maxDurability;
    }
}