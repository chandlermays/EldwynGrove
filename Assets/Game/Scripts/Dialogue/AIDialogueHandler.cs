/*-------------------------
File: AIDialogueHandler.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Core;
using EldwynGrove.Player;

namespace EldwynGrove.Dialogues
{
    public class AIDialogueHandler : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Dialogue m_dialogue;
        [SerializeField] private string m_speakerName;

        public string SpeakerName => m_speakerName;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_dialogue, nameof(m_dialogue));
        }

        /*----------------------------------------------------------------------------
        | --- HandleRaycast: The Behavior of the Raycast for Initiating Dialogue --- |
        ----------------------------------------------------------------------------*/
        public bool HandleRaycast(PlayerController playerController)
        {
   //         if (InputManager.Instance.InputActions.Gameplay.Interact.WasPressedThisFrame())
   //         {
   //             playerController.GetComponent<PlayerDialogueHandler>().BeginDialogueAction(this, m_dialogue);
   //         }

            return true;
        }
    }
}