/*-------------------------
File: QuestItem.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Quests;

namespace EldwynGrove.Inventories
{
    [CreateAssetMenu(fileName = "New Quest Item", menuName = "EldwynGrove/Items/Quest Item", order = 0)]
    public class QuestItem : InventoryItem
    {
        [SerializeField] private Quest m_quest;
        [SerializeField] private QuestObjective m_objective;

        public Quest Quest => m_quest;
        public QuestObjective Objective => m_objective;
    }
}