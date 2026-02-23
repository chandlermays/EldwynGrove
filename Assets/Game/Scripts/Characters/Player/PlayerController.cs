using UnityEngine;
//---------------------------------
using EldwynGrove.Input;
using EldwynGrove.Combat;
using EldwynGrove.Navigation;
using UnityEngine.InputSystem;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Camera m_mainCamera;
        private MovementComponent m_movementComponent;
        private HealthComponent m_healthComponent;
        private EGInputActions m_inputActions;

        private bool m_isTouching;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            m_mainCamera = Camera.main;
            Utilities.CheckForNull(m_mainCamera, nameof(m_mainCamera));

            m_movementComponent = GetComponent<MovementComponent>();
            Utilities.CheckForNull(m_movementComponent, nameof(m_movementComponent));

            m_healthComponent = GetComponent<HealthComponent>();
            Utilities.CheckForNull(m_healthComponent, nameof(m_healthComponent));
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
        | --- Update: Called upon every frame --- |
        -----------------------------------------*/
        private void Update()
        {
            if (!m_isTouching) return;

            Vector2 screenPos = m_inputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
            Vector3 worldPos = m_mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            m_movementComponent.MoveTo(worldPos);
        }

        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            m_isTouching = true;
        }

        private void OnTouchReleased(InputAction.CallbackContext context)
        {
            m_isTouching = false;
        }
    }
}