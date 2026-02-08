using UnityEngine;

namespace EldwynGrove
{
    public class MovementComponent : EntityComponent
    {
        [Header("Movement Settings")]
        [SerializeField] private float m_moveSpeed = 3.0f;

        private Vector2 m_movement;

        public float MoveSpeed => m_moveSpeed;

        private const string kMoveX = "MoveX";
        private const string kMoveY = "MoveY";
        private const string kIsWalking = "IsWalking";

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        protected override void Awake()
        {
            base.Awake();
        }

        /*----------------------------------------------------------------
        | --- FixedUpdate:
        ----------------------------------------------------------------*/
        private void FixedUpdate()
        {
            Rigidbody2D.linearVelocity = m_movement * m_moveSpeed;
        }

        /*----------------------------------------------------------------
        | --- Move:
        ----------------------------------------------------------------*/
        public void Move(Vector2 movement)
        {
            m_movement = movement;

            Animator.SetBool(kIsWalking, true);
            Animator.SetFloat(kMoveX, movement.x);
            Animator.SetFloat(kMoveY, movement.y);
        }

        /*----------------------------------------------------------------
        | --- Stop:
        ----------------------------------------------------------------*/
        public void Stop()
        {
            m_movement = Vector2.zero;

            Animator.SetBool(kIsWalking, false);
        }
    }
}