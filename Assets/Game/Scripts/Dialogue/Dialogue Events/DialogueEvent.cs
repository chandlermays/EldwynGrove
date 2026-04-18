using UnityEngine;
//---------------------------------

namespace EldwynGrove.Dialogues
{
    public abstract class DialogueEvent : ScriptableObject
    {
        public abstract void Execute(GameObject instigator, GameObject target);
    }
}