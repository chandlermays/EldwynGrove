/*-------------------------
File: QuestItem.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Quests;

namespace EldwynGrove.Inventories
{
    [CreateAssetMenu(fileName = "New Quest Item", menuName = "Eldwyn Grove/Items/Quest Item")]
    public class QuestItem : InventoryItem
    {
        [SerializeField] private Quest m_quest;
        [SerializeField] private QuestObjective m_objective;

        public Quest Quest => m_quest;
        public QuestObjective Objective => m_objective;
    }
}