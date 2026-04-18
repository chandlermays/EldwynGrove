/*-------------------------
File: PlayerDialogueHandler.cs
Author: Chandler Mays
-------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;
//---------------------------------
using EldwynGrove.Core;
using EldwynGrove.Tools;
using EldwynGrove.Saving;

namespace EldwynGrove.Dialogues
{
    public class PlayerDialogueHandler : MonoBehaviour, IAction
    {
        private Dialogue m_activeDialogue;
        private DialogueNode m_currentNode;
        private AIDialogueHandler m_activeNPC;

        private bool m_inActiveDialogue = false;

        public event Action OnDialogueStarted;
        public event Action OnDialogueUpdated;
        public event Action OnDialogueEnded;

        private void TriggerEnterAction() => FireEvents(m_currentNode.OnEnterEvents);
        private void TriggerExitAction() => FireEvents(m_currentNode.OnExitEvents);

        /*----------------------------------------------------------- 
        | --- BeginDialogue: Starts the current active Dialogue --- |
        -----------------------------------------------------------*/
        public void BeginDialogue(AIDialogueHandler newNPC, Dialogue newDialogue)
        {
            m_activeNPC = newNPC;
            m_activeDialogue = newDialogue;

            m_inActiveDialogue = true;
            m_currentNode = m_activeDialogue.GetRootNode(GetEvaluators());
            TriggerEnterAction();

            OnDialogueStarted?.Invoke();
            OnDialogueUpdated?.Invoke();
        }

        /*------------------------------------------------------- 
        | --- EndDialogue: Ends the current active Dialogue --- |
        -------------------------------------------------------*/
        public void EndDialogue()
        {
            m_inActiveDialogue = false;
            TriggerExitAction();
            m_activeDialogue = null;
            m_currentNode = null;
            m_activeNPC = null;

            OnDialogueEnded?.Invoke();
            OnDialogueUpdated?.Invoke();
            SaveManager.Instance.Save();        // Auto-save after ending a dialogue
        }

        /*-------------------------------------------------------- 
        | --- IsActive: Returns whether a Dialogue is active --- |
        --------------------------------------------------------*/
        public bool IsActive()
        {
            return m_inActiveDialogue;
        }

        /*------------------------------------------------------------------- 
        | --- Title: Returns the Name of the active NPC in the Dialogue --- |
        -------------------------------------------------------------------*/
        public string GetName()
        {
            return m_activeNPC.SpeakerName;
        }

        /*------------------------------------------------------------- 
        | --- Text: Returns the Text of the current Dialogue Node --- |
        -------------------------------------------------------------*/
        public string GetText()
        {
            if (m_currentNode != null)
            {
                return m_currentNode.Text;
            }

            return "";
        }

        /*----------------------------------------------------------------------- 
        | --- NextDialogueNode: Moves to the next Dialogue Node in the tree --- |
        -----------------------------------------------------------------------*/
        public void NextDialogueNode()
        {
            List<DialogueNode> validChildren = GetValidChildren();
            if (validChildren.Count == 0)
            {
                EndDialogue();
                return;
            }

            int randomIndex = UnityEngine.Random.Range(0, validChildren.Count);
            TriggerExitAction();
            m_currentNode = validChildren[randomIndex];
            TriggerEnterAction();
            OnDialogueUpdated?.Invoke();
        }

        /*------------------------------------------------------------------------------------ 
        | --- HasNextDialogueNode: Checks if there are any children for the current Node --- |
        ------------------------------------------------------------------------------------*/
        public bool HasNextDialogueNode()
        {
            return GetValidChildren().Count > 0;
        }

        /*-----------------------------------------------------
        | --- Cancel: Cancels the current Dialogue action --- |
        -----------------------------------------------------*/
        public void Cancel()
        {
            if (m_inActiveDialogue)
            {
                EndDialogue();
                return;
            }

            m_currentNode = null;
            m_activeNPC = null;
            m_activeDialogue = null;
        }

        /*------------------------------------------------------------------------------------ 
        | --- GetValidChildren: Returns a list of child nodes that meet their conditions --- |
        ------------------------------------------------------------------------------------*/
        private List<DialogueNode> GetValidChildren()
        {
            // Recompute the valid children every time to ensure conditions are checked against the latest state.
            List<DialogueNode> validChildren = new();
            IEnumerable<IConditionChecker> evaluators = GetEvaluators();

            foreach (DialogueNode child in m_activeDialogue.GetAllChildren(m_currentNode))
            {
                if (child.CheckCondition(evaluators))
                {
                    validChildren.Add(child);
                }
            }

            return validChildren;
        }

        /*------------------------------------------------------------------------- 
        | --- GetEvaluators: Returns all Condition Checkers on the active NPC --- |
        -------------------------------------------------------------------------*/
        private IEnumerable<IConditionChecker> GetEvaluators()
        {
            return GetComponents<IConditionChecker>();
        }

        /*----------------------------------------------------------------------- 
        | --- FireEvents: Executes a list of OnEnter/OnExit dialogue events --- |
        -----------------------------------------------------------------------*/
        private void FireEvents(IReadOnlyList<DialogueEvent> events)
        {
            foreach (DialogueEvent dialogueEvent in events)
            {
                dialogueEvent?.Execute(gameObject, m_activeNPC.gameObject);
            }
        }
    }
}