using UnityEngine;
using UnityEngine.InputSystem;
//---------------------------------
using EldwynGrove.Input;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour, EGInputActions.IGameplayActions
    {
        private Animator m_animator;
        private Rigidbody2D m_rigidody2D;
        private MovementComponent m_movementComponent;

        private EGInputActions m_inputActions;

        private Vector2 m_direction;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            Utilities.CheckForNull(m_animator, nameof(m_animator));

            m_rigidody2D = GetComponent<Rigidbody2D>();
            Utilities.CheckForNull(m_rigidody2D, nameof(m_rigidody2D));

            m_movementComponent = GetComponent<MovementComponent>();
            Utilities.CheckForNull(m_movementComponent, nameof(m_movementComponent));

            m_inputActions = new EGInputActions();
            Utilities.CheckForNull(m_inputActions, nameof(m_inputActions));
        }

        /*---------------------------------------------------------------------
        | --- OnEnable: Called when the object becomes enabled and active --- |
        ---------------------------------------------------------------------*/
        private void OnEnable()
        {
            m_inputActions.Gameplay.SetCallbacks(this);
            m_inputActions.Gameplay.Enable();
        }

        /*---------------------------------------------------------------------------
        | --- OnDisable: Called when the behaviour becomes disabled or inactive --- |
        ---------------------------------------------------------------------------*/
        private void OnDisable()
        {
            m_inputActions.Gameplay.Disable();
        }

        /*----------------------------------------------------------------------
        | --- OnMove: Called when the Move action is performed or canceled --- |
        ----------------------------------------------------------------------*/
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                m_movementComponent.Stop();
            }
            else
            {
                Vector2 movement = context.ReadValue<Vector2>();
                if (movement != Vector2.zero)
                {
                    m_direction = movement.normalized;
                }
                m_movementComponent.Move(movement);
            }
        }
    }
}