using UnityEngine;
//---------------------------------
using EldwynGrove.Quests;

namespace EldwynGrove.Dialogues
{
    [CreateAssetMenu(menuName = "Eldwyn Grove/Dialogue/Events/Assign Quest", fileName = "New Assign Quest Event")]
    public class AssignQuestEvent : DialogueEvent
    {
        [SerializeField] private Quest m_quest;

        /*--------------------------------------------------------------------------
        | --- Execute: Assigns the configured quest to the player (instigator) --- |
        --------------------------------------------------------------------------*/
        public override void Execute(GameObject instigator, GameObject target)
        {
            instigator.GetComponent<QuestManager>().AddQuest(m_quest);
        }
    }
}