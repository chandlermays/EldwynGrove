using UnityEngine;
//---------------------------------
using EldwynGrove.Input;
using EldwynGrove.Combat;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float m_moveSpeed = 5.0f;

        private Animator m_animator;
        private Rigidbody2D m_rigidody2D;
        private HealthComponent m_healthComponent;
        private EGInputActions m_inputActions;

        private bool m_isMoving;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            Utilities.CheckForNull(m_animator, nameof(m_animator));

            m_rigidody2D = GetComponent<Rigidbody2D>();
            Utilities.CheckForNull(m_rigidody2D, nameof(m_rigidody2D));

            m_healthComponent = GetComponent<HealthComponent>();
            Utilities.CheckForNull(m_healthComponent, nameof(m_healthComponent));
        }

        /*-----------------------------------------------------
        | --- Start: Called before the first frame update --- |
        -----------------------------------------------------*/
        private void Start()
        {
            m_inputActions = InputManager.Instance.InputActions;
        }

        /*-----------------------------------------
        | --- Update: Called upon every frame --- |
        -----------------------------------------*/
        private void Update()
        {

        }
    }
}