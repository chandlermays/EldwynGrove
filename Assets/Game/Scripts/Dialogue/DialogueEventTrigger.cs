/*-------------------------
File: DialogueEventTrigger.cs
Author: Chandler Mays
-------------------------*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//---------------------------------

namespace EldwynGrove.Dialogues
{
    [System.Serializable]
    public class ActionTriggerPair
    {
        [SerializeField] private string m_action;
        [SerializeField] private UnityEvent OnTrigger;

        public string Action => m_action;
        public UnityEvent TriggerEvent => OnTrigger;
    }

    public class DialogueEventTrigger : MonoBehaviour
    {
        [SerializeField] private List<ActionTriggerPair> m_actionTriggerPairs = new();

        /*----------------------------------------------------------------------------
        | --- Trigger: Triggers the UnityEvent associated with the action string --- |
        ----------------------------------------------------------------------------*/
        public void Trigger(string action)
        {
            foreach (var pair in m_actionTriggerPairs)
            {
                if (pair.Action == action)
                {
                    pair.TriggerEvent?.Invoke();
                    return;
                }
            }
        }
    }
}