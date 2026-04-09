/*-------------------------
File: DialogueUI.cs
Author: Chandler Mays
-------------------------*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//---------------------------------

// Future Feature: Add support for dialogue choices and complex branching paths.

namespace EldwynGrove.Dialogues
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_dialogueSpeaker;
        [SerializeField] private TextMeshProUGUI m_dialogueText;
        [SerializeField] private Button m_nextButton;
        [SerializeField] private Button m_endButton;
        [SerializeField] private Button m_quitButton;
        [SerializeField] private PlayerDialogueHandler m_playerDialogueHandler;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_dialogueSpeaker, nameof(m_dialogueSpeaker));
            Utilities.CheckForNull(m_dialogueText, nameof(m_dialogueText));
            Utilities.CheckForNull(m_nextButton, nameof(m_nextButton));
            Utilities.CheckForNull(m_endButton, nameof(m_endButton));
            Utilities.CheckForNull(m_quitButton, nameof(m_quitButton));
            Utilities.CheckForNull(m_playerDialogueHandler, nameof(m_playerDialogueHandler));

            m_nextButton.onClick.AddListener(m_playerDialogueHandler.NextDialogueNode);
            m_endButton.onClick.AddListener(m_playerDialogueHandler.EndDialogue);
            m_quitButton.onClick.AddListener(m_playerDialogueHandler.EndDialogue);
        }

        /*---------------------------------------------------------------------
        | --- OnEnable: Called when the object becomes enabled and active --- |
        ---------------------------------------------------------------------*/
        private void OnEnable()
        {
            m_playerDialogueHandler.OnDialogueUpdated += UpdateUI;
        }

        /*--------------------------------------------------------------------
        | --- OnDestroy: Called when the MonoBehaviour will be destroyed --- |
        --------------------------------------------------------------------*/
        private void OnDestroy()
        {
            m_playerDialogueHandler.OnDialogueUpdated -= UpdateUI;

            m_nextButton.onClick.RemoveListener(m_playerDialogueHandler.NextDialogueNode);
            m_endButton.onClick.RemoveListener(m_playerDialogueHandler.EndDialogue);
            m_quitButton.onClick.RemoveListener(m_playerDialogueHandler.EndDialogue);
        }

        /*-----------------------------------------------------
        | --- Start: Called before the first frame update --- |
        -----------------------------------------------------*/
        void Start()
        {
            gameObject.SetActive(m_playerDialogueHandler.IsActive());
        }

        /*-----------------------------------------------------------------
        | --- UpdateUI: Updates the UI with the current Dialogue Text --- |
        -----------------------------------------------------------------*/
        private void UpdateUI()
        {
            if (!m_playerDialogueHandler.IsActive())
                return;

            m_dialogueSpeaker.text = m_playerDialogueHandler.GetName();
            m_dialogueText.text = m_playerDialogueHandler.GetText();
            m_nextButton.gameObject.SetActive(m_playerDialogueHandler.HasNextDialogueNode());
            m_endButton.gameObject.SetActive(!m_playerDialogueHandler.HasNextDialogueNode());
        }
    }
}