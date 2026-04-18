using UnityEngine;
//---------------------------------
using EldwynGrove.Quests;

namespace EldwynGrove.Dialogues
{
    [CreateAssetMenu(menuName = "Eldwyn Grove/Dialogue/Events/Complete Objective", fileName = "New Complete Objective Event")]
    public class CompleteObjectiveEvent : DialogueEvent
    {
        [SerializeField] private Quest m_quest;
        [SerializeField] private QuestObjective m_objective;

        /*-----------------------------------------------------------------------------------------------------------------
        | --- Execute: Marks the configured objective of the configured quest as complete for the player (instigator) --- |
        -----------------------------------------------------------------------------------------------------------------*/
        public override void Execute(GameObject instigator, GameObject target)
        {
            instigator.GetComponent<QuestManager>().CompleteObjective(m_quest, m_objective);
        }
    }
}