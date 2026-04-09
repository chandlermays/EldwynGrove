/*-------------------------
File: QuestObjective.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Quests
{
    [CreateAssetMenu(fileName = "New Quest Objective", menuName = "EldwynGrove/Quests/Quest Objective", order = 0)]
    public class QuestObjective : ScriptableObject
    {
        [SerializeField, TextArea] private string m_description;    // do I really want this property?
        public string Description => m_description;

        public static QuestObjective GetByName(string objectiveName)
        {
            foreach (QuestObjective objective in Resources.LoadAll<QuestObjective>(""))
            {
                if (objective.name == objectiveName) return objective;
            }
            return null;
        }
    }
}