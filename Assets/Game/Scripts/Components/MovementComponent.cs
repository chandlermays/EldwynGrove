/*-------------------------
File: MovementComponent.cs
Author: Chandler Mays
-------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//---------------------------------
using EldwynGrove.Navigation;

namespace EldwynGrove.Components
{
    public class MovementComponent : EntityComponent
    {
        [SerializeField] private VisionCone m_visionCone;
        [SerializeField] private float m_moveSpeed = 5f;

        private static readonly int s_animMoveX = Animator.StringToHash("MoveX");
        private static readonly int s_animMoveY = Animator.StringToHash("MoveY");
        private static readonly int s_animIsMoving = Animator.StringToHash("IsMoving");

        private Vector2 m_lastDirection = Vector2.down;
        private Coroutine m_moveCoroutine;

        public bool IsMoving { get; private set; }
        public Vector2 LastDirection => m_lastDirection;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        protected override void Awake()
        {
            base.Awake();

            SetDirection(Vector2.right);
        }

        /*----------------------------------------------------------------------------------
        | --- Move: Applies movement input to the rigidbody and updates movement state --- |
        ----------------------------------------------------------------------------------*/
        public void Move(Vector2 input)
        {
            if (m_moveCoroutine != null)
            {
                // Joystick input interrupts any active path movement
                if (input.sqrMagnitude > 0.0001f)
                    Stop();
                else
                    return;
            }

            Vector2 moveDirection = input.sqrMagnitude > 1f ? input.normalized : input;

            Rigidbody2D.linearVelocity = moveDirection * m_moveSpeed;

            IsMoving = moveDirection.sqrMagnitude > 0.0001f;
            Animator.SetBool(s_animIsMoving, IsMoving);

            if (IsMoving)
            {
                SetDirection(moveDirection);
                m_visionCone?.SetDirection(moveDirection);
            }
        }

        /*--------------------------------------------------------------
        | --- MoveTo: Initiates movement towards a target position --- |
        --------------------------------------------------------------*/
        public bool MoveTo(Vector3 targetPos, Action onComplete = null)
        {
            List<Vector3> path = Pathfinder.FindPath(Transform.position, targetPos);
            if (path == null)
            {
                Debug.LogWarning("No path found to target position.");
                return false;
            }

            // Already at the destination — path is valid but empty
            if (path.Count == 0)
            {
                onComplete?.Invoke();
                return true;
            }

            if (m_moveCoroutine != null)
            {
                StopCoroutine(m_moveCoroutine);
                m_moveCoroutine = null;
            }

            m_moveCoroutine = StartCoroutine(MoveAlongPath(path, onComplete));
            return true;
        }

        /*-----------------------------------------------------------------------------------------
        | --- SetDirection: Updates animator parameters to reflect current movement direction --- |
        -----------------------------------------------------------------------------------------*/
        public void SetDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude > 0f)
            {
                m_lastDirection = GetCardinalDirection(direction);
            }

            Animator.SetFloat(s_animMoveX, m_lastDirection.x);
            Animator.SetFloat(s_animMoveY, m_lastDirection.y);
        }

        /*---------------------------------------------------------------------
        | --- Stop: Halts any ongoing movement and resets animation state --- |
        ---------------------------------------------------------------------*/
        public void Stop()
        {
            if (m_moveCoroutine != null)
            {
                StopCoroutine(m_moveCoroutine);
                m_moveCoroutine = null;
            }

            Rigidbody2D.linearVelocity = Vector2.zero;
            SetMoving(false, m_lastDirection);
        }

        /*-----------------------------------------------------------------------------
        | --- MoveAlongPath: Coroutine that moves the entity along the given path --- |
        -----------------------------------------------------------------------------*/
        private IEnumerator MoveAlongPath(List<Vector3> path, Action onComplete)
        {
            Rigidbody2D.linearVelocity = Vector2.zero;
            SetMoving(true, m_lastDirection);

            foreach (Vector3 waypoint in path)
            {
                Vector3 target = new(waypoint.x, waypoint.y, Transform.position.z);
                Vector2 direction = GetCardinalDirection(target - Transform.position);

                SetMoving(true, direction);

                while (Vector3.Distance(Transform.position, target) > 0.01f)
                {
                    Transform.position = Vector3.MoveTowards(Transform.position, target, m_moveSpeed * Time.deltaTime);
                    yield return null;
                }

                Transform.position = target;
            }

            SetMoving(false, m_lastDirection);
            m_moveCoroutine = null;

            onComplete?.Invoke();
        }

        /*-------------------------------------------------------------------
        | --- SetMoving: Updates movement state and animator parameters --- |
        -------------------------------------------------------------------*/
        private void SetMoving(bool isMoving, Vector2 direction)
        {
            if (direction.sqrMagnitude > 0f)
            {
                m_lastDirection = GetCardinalDirection(direction);
            }

            IsMoving = isMoving;
            Animator.SetBool(s_animIsMoving, isMoving);
            Animator.SetFloat(s_animMoveX, m_lastDirection.x);
            Animator.SetFloat(s_animMoveY, m_lastDirection.y);
            m_visionCone?.SetDirection(m_lastDirection);
        }

        /*----------------------------------------------------------------------------------
        | --- GetCardinalDirection: Snaps a direction to one of the four cardinal axes --- |
        ----------------------------------------------------------------------------------*/
        private Vector2 GetCardinalDirection(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x >= 0f ? Vector2.right : Vector2.left;
            }

            return direction.y >= 0f ? Vector2.up : Vector2.down;
        }
    }
}