/*-------------------------
File: QuestAssigner.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Quests
{
    public class QuestAssigner : MonoBehaviour
    {
        [SerializeField] private QuestManager m_playerQuestMgr;
        [SerializeField] private Quest m_quest;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_playerQuestMgr, nameof(m_playerQuestMgr));
            Utilities.CheckForNull(m_quest, nameof(m_quest));
        }

        /*-----------------------------------------------------
        | --- AssignQuest: Assign the quest to the player --- |
        -----------------------------------------------------*/
        public void AssignQuest()
        {
            m_playerQuestMgr.AddQuest(m_quest);
        }
    }
}