/*-------------------------
File: QuestUI.cs
Author: Chandler Mays
-------------------------*/
using TMPro;
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Quests
{
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_questTitle;
        [SerializeField] private TextMeshProUGUI m_questProgress;

        private QuestStatus m_status;
        public QuestStatus GetQuestStatus() => m_status;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_questTitle, nameof(m_questTitle));
            Utilities.CheckForNull(m_questProgress, nameof(m_questProgress));
        }

        /*------------------------------------------------
        | --- Setup: Initialize the quest UI element --- |
        ------------------------------------------------*/
        public void Setup(QuestStatus status)
        {
            m_status = status;
            Quest quest = status.Quest;

            m_questTitle.text = quest.Title;

            // Format the progression text
            m_questProgress.text = status.CompletedObjectiveCount + "/" + quest.ObjectiveCount;
        }
    }
}