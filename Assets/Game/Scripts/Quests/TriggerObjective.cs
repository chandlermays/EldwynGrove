/*-------------------------
File: TriggerObjective.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Quests
{
    [RequireComponent(typeof(Collider))]
    public class TriggerObjective : MonoBehaviour
    {
        private const string kPlayerTag = "Player";

        /*-------------------------------------------------------------------------
        | --- OnTriggerEnter: Called when another collider enters the trigger --- |
        -------------------------------------------------------------------------*/
        private void OnTriggerEnter(Collider other)
        {
            // Check if the Player entered the trigger
            if (other.CompareTag(kPlayerTag))
            {
                // If the component exists, complete the objective
                if (TryGetComponent<ObjectiveCompletion>(out var questCompletion))
                {
                    questCompletion.CompleteObjective();
                }
            }
        }
    }
}