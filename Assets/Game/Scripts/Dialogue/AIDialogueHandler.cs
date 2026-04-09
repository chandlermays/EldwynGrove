/*-------------------------
File: AIDialogueHandler.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Core;
using EldwynGrove.Input;
using EldwynGrove.Player;

namespace EldwynGrove.Dialogues
{
    public class AIDialogueHandler : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Dialogue m_dialogue;
        [SerializeField] private string m_speakerName;

        public string SpeakerName => m_speakerName;

        private Outline m_outline;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_dialogue, nameof(m_dialogue));

            m_outline = GetComponent<Outline>();
            Utilities.CheckForNull(m_outline, nameof(m_outline));
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

        /*-------------------------------------------------------------------------------------
        | --- ToggleHighlight: Enables or Disables the Outline Component for Highlighting --- |
        -------------------------------------------------------------------------------------*/
        public void ToggleHighlight(bool highlight)
        {
            m_outline.enabled = highlight;
        }
    }
}