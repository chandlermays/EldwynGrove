/*-------------------------
File: ToolDurability.cs
Author: Chandler Mays
-------------------------*/
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
//---------------------------------
using EldwynGrove.Saving;

namespace EldwynGrove.Inventories
{
    [RequireComponent(typeof(Equipment))]
    [RequireComponent(typeof(Inventory))]
    public class ToolDurability : MonoBehaviour, ISaveable
    {
        private Equipment m_equipment;
        private Inventory m_inventory;

        private readonly Dictionary<EquipmentSlot, int> m_durability = new();

        public event Action<EquipmentSlot, int> OnDurabilityChanged;

        private void Awake()
        {
            m_equipment = GetComponent<Equipment>();
            m_inventory = GetComponent<Inventory>();
        }

        private void OnEnable()
        {
            m_equipment.OnEquipmentChanged += HandleEquipmentChanged;
        }

        private void OnDisable()
        {
            m_equipment.OnEquipmentChanged -= HandleEquipmentChanged;
        }

        public int GetCurrentDurability(EquipmentSlot slot)
        {
            return m_durability.TryGetValue(slot, out int val) ? val : -1;
        }

        public void UseTool(EquipmentSlot slot)
        {
            if (!m_durability.ContainsKey(slot))
                return;

            --m_durability[slot];
            OnDurabilityChanged?.Invoke(slot, m_durability[slot]);

            if (m_durability[slot] <= 0)
            {
                BreakTool(slot);
            }
        }

        private void HandleEquipmentChanged()
        {
            var slotsToRemove = new List<EquipmentSlot>();

            foreach (var slot in m_durability.Keys)
            {
                if (m_equipment.GetItemInSlot(slot) is not ToolItem)
                {
                    slotsToRemove.Add(slot);
                }
            }

            foreach (var slot in slotsToRemove)
            {
                m_durability.Remove(slot);
            }

            foreach (var slot in m_equipment.OccupiedSlots)
            {
                if (m_equipment.GetItemInSlot(slot) is ToolItem tool && !m_durability.ContainsKey(slot))
                {
                    m_durability[slot] = tool.MaxDurability;
                }
            }
        }

        private void BreakTool(EquipmentSlot slot)
        {
            EquipableItem brokenTool = m_equipment.GetItemInSlot(slot);
            m_durability.Remove(slot);
            m_equipment.RemoveItem(slot);

            for (int i = 0; i < m_inventory.Size; i++)
            {
                if (m_inventory.GetItemAtSlot(i) == brokenTool)
                {
                    m_inventory.RemoveItemsFromSlot(i, 1);
                    break;
                }
            }

            SaveManager.Instance.Save();
        }

        public JToken CaptureState()
        {
            throw new NotImplementedException();
        }

        public void RestoreState(JToken state)
        {
            throw new NotImplementedException();
        }
    }
}