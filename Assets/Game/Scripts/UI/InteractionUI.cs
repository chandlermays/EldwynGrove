/*-------------------------
File: InteractionUI.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Components;
using EldwynGrove.Core;

namespace EldwynGrove.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private VisionCone m_visionCone;

        private CanvasGroup m_canvasGroup;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_visionCone, nameof(m_visionCone));

            m_canvasGroup = GetComponent<CanvasGroup>();
            Utilities.CheckForNull(m_canvasGroup, nameof(m_canvasGroup));

            SetPromptVisible(false);
        }

        /*---------------------------------------------------------------------
        | --- OnEnable: Subscribe to vision cone events --- |
        ---------------------------------------------------------------------*/
        private void OnEnable()
        {
            m_visionCone.OnInteractableEntered += OnInteractableEntered;
            m_visionCone.OnInteractableExited += OnInteractableExited;
        }

        /*----------------------------------------------------------------------
        | --- OnDisable: Unsubscribe from vision cone events --- |
        ----------------------------------------------------------------------*/
        private void OnDisable()
        {
            m_visionCone.OnInteractableEntered -= OnInteractableEntered;
            m_visionCone.OnInteractableExited -= OnInteractableExited;
        }

        private void OnInteractableEntered(IRaycastable _)
        {
            SetPromptVisible(true);
        }

        private void OnInteractableExited(IRaycastable _)
        {
            SetPromptVisible(m_visionCone.HasInteractable);
        }

        private void SetPromptVisible(bool visible)
        {
            m_canvasGroup.alpha = visible ? 1f : 0f;
            m_canvasGroup.interactable = visible;
            m_canvasGroup.blocksRaycasts = visible;
        }
    }
}