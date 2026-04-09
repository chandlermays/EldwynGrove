/*-------------------------
File: ObjectiveCompletion.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Quests
{
    public class ObjectiveCompletion : MonoBehaviour
    {
        [SerializeField] private QuestManager m_questManager;
        [SerializeField] private Quest m_quest;
        [SerializeField] private QuestObjective m_objective;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_questManager, nameof(m_questManager));
            Utilities.CheckForNull(m_quest, nameof(m_quest));
            Utilities.CheckForNull(m_objective, nameof(m_objective));
        }

        /*-----------------------------------------------------------------
        | --- CompleteObjective: Mark a quest's objective as complete --- |
        -----------------------------------------------------------------*/
        public void CompleteObjective()
        {
            m_questManager.CompleteObjective(m_quest, m_objective);
        }
    }
}