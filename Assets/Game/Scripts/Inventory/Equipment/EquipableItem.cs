/*-------------------------
File: EquipableItem.cs
Author: Chandler Mays
-------------------------*/
using EldwynGrove.Tools;
//---------------------------------
using UnityEngine;

namespace EldwynGrove.Inventories
{
    [CreateAssetMenu(menuName = "EldwynGrove/Items/Equipable Item", fileName = "New Equipable Item", order = 0)]
    public class EquipableItem : InventoryItem
    {
        [Tooltip("The equipment slot this m_item can be equipped to.")]
        [SerializeField] private EquipmentSlot m_targetEquipmentSlot = EquipmentSlot.kNone;
        [SerializeField] private Condition m_equipCondition;

        public EquipmentSlot TargetEquipmentSlot => m_targetEquipmentSlot;

        /*---------------------------------------------------------------------------
        | --- CanEquip: Check if the item can be equipped to the specified slot --- |
        ---------------------------------------------------------------------------*/
        public bool CanEquip(EquipmentSlot equipmentSlot, Equipment equipment)
        {
            if (m_targetEquipmentSlot != equipmentSlot)
                return false;

            return m_equipCondition.Check(equipment.GetComponents<IConditionChecker>());
        }
    }
}