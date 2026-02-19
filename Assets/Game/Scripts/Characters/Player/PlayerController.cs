using UnityEngine;
using UnityEngine.InputSystem;
//---------------------------------
using EldwynGrove.Navigation;
using EldwynGrove.Input;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MyJoystick m_joystick;

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
    }
}