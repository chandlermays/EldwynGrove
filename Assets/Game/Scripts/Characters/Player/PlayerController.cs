using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
//---------------------------------
using EldwynGrove.Input;
using EldwynGrove.Navigation;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TileCursor m_tileCursor;

        private Camera m_mainCamera;
        private EGInputActions m_inputActions;
        private PlayerInteractionHandler m_interactionHandler;
        private bool m_isTouching;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_tileCursor, nameof(m_tileCursor));
            m_mainCamera = Camera.main;

            m_interactionHandler = GetComponent<PlayerInteractionHandler>();
            Utilities.CheckForNull(m_interactionHandler, nameof(m_interactionHandler));

            m_interactionHandler.Initialize(
                m_tileCursor,
                GetComponent<MovementComponent>(),
                GetComponent<GatheringComponent>()
            );
        }

        /*-----------------------------------------------------
        | --- Start: Called before the first frame update --- |
        -----------------------------------------------------*/
        private void Start()
        {
            m_inputActions = InputManager.Instance.InputActions;
            m_inputActions.Gameplay.TouchPress.performed += OnTouchStarted;
            m_inputActions.Gameplay.TouchPress.canceled += OnTouchReleased;
        }

        /*--------------------------------------------------------------------
        | --- OnDestroy: Called when the MonoBehaviour will be destroyed --- |
        --------------------------------------------------------------------*/
        private void OnDestroy()
        {
            m_inputActions.Gameplay.TouchPress.performed -= OnTouchStarted;
            m_inputActions.Gameplay.TouchPress.canceled -= OnTouchReleased;
        }

        /*-----------------------------------------
        | --- Update: Called once every frame --- |
        -----------------------------------------*/
        private void Update()
        {
            if (!m_isTouching)
                return;

            if (Touch.activeFingers.Count >= 2)
            {
                m_isTouching = false;
                return;
            }

            Vector2 screenPos = m_inputActions.Gameplay.TouchPosition.ReadValue<Vector2>();

            if (IsTouchOverUI(screenPos))
                return;

            Vector3 worldPos = m_mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            m_interactionHandler.HandleInteractAt(worldPos);
        }

        /*-----------------------------------------------------------------
        | --- IsTouchOverUI: Checks if the touch is over a UI element --- |
        -----------------------------------------------------------------*/
        private bool IsTouchOverUI(Vector2 screenPos)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = screenPos };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            return results.Count > 0;
        }

        private void OnTouchStarted(InputAction.CallbackContext context) => m_isTouching = true;
        private void OnTouchReleased(InputAction.CallbackContext context) => m_isTouching = false;
    }
}