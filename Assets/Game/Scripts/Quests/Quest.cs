/*-------------------------
File: Quest.cs
Author: Chandler Mays
-------------------------*/
using System.Collections.Generic;
using UnityEngine;
//---------------------------------
using EldwynGrove.Inventories;

namespace EldwynGrove.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Eldwyn Grove/Quests/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<QuestObjective> m_objectives = new();
        [SerializeField] private List<Reward> m_rewards = new();

        [System.Serializable]
        public class Reward
        {
            [SerializeField] private InventoryItem m_item;
            [Min(1)][SerializeField] private int m_amount;

            public InventoryItem Item => m_item;
            public int Amount => m_amount;
        }

        public string Title => name;
        public IReadOnlyList<QuestObjective> Objectives => m_objectives;
        public int ObjectiveCount => m_objectives.Count;
        public List<Reward> Rewards => m_rewards;

        /*---------------------------------------------------------------------------
        | --- GetByName: Retrieve a Quest by its name from the Resources folder --- |
        ---------------------------------------------------------------------------*/
        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }

        /*-------------------------------------------------------------------------------
        | --- HasObjective: Check if the quest has a specific objective by its name --- |
        -------------------------------------------------------------------------------*/
        public bool HasObjective(QuestObjective objective)
        {
            return m_objectives.Contains(objective);
        }
    }
}